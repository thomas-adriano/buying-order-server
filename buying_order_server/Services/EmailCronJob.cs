using buying_order_server.Contracts;
using buying_order_server.Data.Entity;
using buying_order_server.DTO.Response;
using buying_order_server.Exceptions;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace buying_order_server.Services
{

    public struct EmailSchedulerConfigs
    {
        public string cron;
        public SmtpConfigs smtpConfigs;
        public IEnumerable<EmailConfigs> emailConfigs;
    }

    public struct SmtpConfigs
    {
        public string smtpHost;
        public int smtpPort;
        public bool useSSL;
        public string smtpUser;
        public string smtpPass;

    }

    public struct EmailConfigs
    {
        public string senderEmail;
        public string senderName;
        public string destinationEmail;
        public string subject;
        public string htmlContent;
        public string textContent;
    }


    public class EmailCronJob : AbstractCronJob
    {
        private AppConfiguration _dbConfigs;
        private IAppConfigurationRepository _appConfigsRepo;
        private IBuyingOrdersManager _ordersManager;
        private IConfiguration _config;
        private int _executionCount = 0;
        private IAppExecutionStatusManager _executionStatusManger;
        private SmtpClient _smtpClient;
        private IWebHostEnvironment _env;


        public EmailCronJob(ILogger<AbstractCronJob> logger, IAppConfigurationRepository appConfigsRepo, IBuyingOrdersManager ordersManager, IConfiguration config, IAppExecutionStatusManager executionStatusManger, IWebHostEnvironment env) : base(logger, executionStatusManger)
        {
            _appConfigsRepo = appConfigsRepo;
            _ordersManager = ordersManager;
            _config = config;
            _executionStatusManger = executionStatusManger;
            _env = env;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogDebug("executing email sending routine");
            await StartScheduler(cancellationToken);
        }
        protected override async Task StopWork()
        {
            if (_smtpClient != null)
            {
                try
                {
                    await _smtpClient.DisconnectAsync(true);
                    _smtpClient.Dispose();
                    _smtpClient = null;
                }
                catch (Exception e)
                {
                    _logger.LogError("error trying to disconnect smpt client");
                }

            }
            _executionCount = 0;
        }

        public override async Task<string> CronPattern()
        {
            return await _appConfigsRepo.GetCronPattern();
        }

        private async Task StartScheduler(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            _dbConfigs = await _appConfigsRepo.GetLastAsync();
            var smtpCfg = new SmtpConfigs { smtpHost = _dbConfigs.AppSMTPAddress, smtpPort = _dbConfigs.AppSMTPPort, useSSL = _dbConfigs.AppSMTPSecure, smtpUser = _dbConfigs.AppEmailUser, smtpPass = _dbConfigs.AppEmailPassword };
            var emailSchedulerCfg = new EmailSchedulerConfigs { cron = _dbConfigs.AppCronPattern, smtpConfigs = smtpCfg };
            _executionStatusManger.ChangeExecutionStatus(DTO.AppExecutionStatuses.SchedulerRunning);
            var ordersAndProviders = await _ordersManager.getBuyingOrdersAsync(cancellationToken);
            if (ordersAndProviders == null)
            {
                return;
            }



            emailSchedulerCfg.emailConfigs = ordersAndProviders.Select(e =>
              {
                  var textContent = this.interpolateVariables(_dbConfigs.AppEmailText, e.Order, false);
                  var htmlContent = this.interpolateVariables(_dbConfigs.AppEmailHtml, e.Order, true);
                  var subject = this.interpolateVariables(_dbConfigs.AppEmailSubject, e.Order, false);
                  return new EmailConfigs
                  {
                      htmlContent = htmlContent,
                      textContent = textContent,
                      subject = subject,
                      senderName = _dbConfigs.AppEmailName,
                      senderEmail = _dbConfigs.AppEmailFrom,
                      destinationEmail = e.Provider.Email
                  };
              }
            );
            await StartSendingEmailsAsync(emailSchedulerCfg.smtpConfigs, emailSchedulerCfg.emailConfigs, cancellationToken);
            _executionCount++;
            _logger.LogInformation($"CRON job {emailSchedulerCfg.cron} cycle {_executionCount} successfully executed!");
        }

        private async Task StartSendingEmailsAsync(SmtpConfigs smtpCfgs, IEnumerable<EmailConfigs> emailCfgs, CancellationToken cancellationToken)
        {
            int emailSendingCounter = 1;
            using (var smtpClient = new SmtpClient())
            {
                _smtpClient = smtpClient;
                smtpClient.Connect(smtpCfgs.smtpHost, smtpCfgs.smtpPort, smtpCfgs.useSSL);
                _logger.LogInformation($"SMTP connected");
                smtpClient.Authenticate(smtpCfgs.smtpUser, smtpCfgs.smtpPass);
                _logger.LogInformation($"SMTP authenticated");
                foreach (EmailConfigs cfg in emailCfgs)
                {
                    _logger.LogInformation($"Sending e-mail {emailSendingCounter} of {emailCfgs.Count()} to {cfg.destinationEmail}");
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        await SendEmailAsync(smtpClient, cfg, cancellationToken);
                        _logger.LogInformation($"E-mail {emailSendingCounter} of {emailCfgs.Count()} successfully sent to {cfg.destinationEmail}");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"E-mail {emailSendingCounter} of {emailCfgs.Count()} could not be sent to {cfg.destinationEmail}", e);
                        throw e;
                    }
                    finally
                    {
                        emailSendingCounter++;
                    }
                }
                await smtpClient.DisconnectAsync(true);
            }
            _smtpClient = null;

        }

        private async Task SendEmailAsync(SmtpClient smtpClient, EmailConfigs configs, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Sending e-mail to {configs.destinationEmail}");
                var mimeMsg = new MimeMessage();
                mimeMsg.From.Add(new MailboxAddress(configs.senderName, configs.senderEmail));
                if (bool.Parse(_config["Email:UseTestRecipient"]) || _env.IsDevelopment())
                {
                    mimeMsg.To.Add(new MailboxAddress(_config["Email:TestRecipient"]));
                }
                else
                {
                    mimeMsg.To.Add(new MailboxAddress(configs.destinationEmail));
                }
                mimeMsg.Subject = configs.subject;
                var bb = new BodyBuilder
                {
                    HtmlBody = configs.htmlContent,
                    TextBody = configs.textContent
                };
                mimeMsg.Body = bb.ToMessageBody();

                await smtpClient.SendAsync(mimeMsg, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"There was an error trying to send e-mail to {configs.destinationEmail}", e);
            }
        }

        private string interpolateVariables(string content, BuyingOrdersResponse order, bool html)
        {
            var ret = new Regex(@"\$\{providerName\}")
                .Replace(content, order.NomeContato);
            ret = new Regex(@"\$\{orderNumber\}")
                .Replace(ret, order.NumeroPedido);
            ret = new Regex(@"\$\{orderDate\}")
                .Replace(ret, order.Data);
            ret = new Regex(@"\$\{previewOrderDate\}")
                .Replace(ret, order.DataPrevista);
            ret = new Regex(@"\$\{orderContactName\}")
                .Replace(ret, order.NomeContato);

            if (this._dbConfigs.AppReplyLink != null)
            {
                var link = "";
                if (_env.IsDevelopment())
                {
                    link = $"http://localhost:4201?orderid={order.Id}";
                }
                else
                {
                    link = $"{this._dbConfigs.AppReplyLink}?orderid={order.Id}";
                }
                if (html)
                {
                    ret = new Regex(@"\$\{replyLinkBegin\}")
                        .Replace(ret, $"<a href=\"{link}\" target=\"_blank\">");
                    ret = new Regex(@"\$\{replyLinkEnd\}")
                        .Replace(ret, $"</a>");
                }
                else
                {
                    ret = new Regex(@"\$\{replyLink\}")
                        .Replace(ret, $"Link para informar uma nova data: {link}");
                }

            }

            return ret;
        }
    }
}
