using FluentValidation;
using System;

namespace buying_order_server.DTO.Request
{
    public class CreateOrUpdateAppConfigurationRequest
    {
        public string AppEmailName { get; set; }
        public string AppEmailUser { get; set; }
        public string AppEmailPassword { get; set; }
        public string AppBlacklist { get; set; }
        public string AppSMTPAddress { get; set; }
        public int AppSMTPPort { get; set; }
        public bool AppSMTPSecure { get; set; }
        public string AppEmailFrom { get; set; }
        public string AppEmailSubject { get; set; }
        public string AppEmailText { get; set; }
        public string AppEmailHtml { get; set; }
        public string AppCronPattern { get; set; }
        public string AppReplyLink { get; set; }
        public int AppNotificationTriggerDelta { get; set; }
    }

    public class CreateOrUpdateAppConfigurationRequestValidator : AbstractValidator<CreateOrUpdateAppConfigurationRequest>
    {
        public CreateOrUpdateAppConfigurationRequestValidator()
        {
            RuleFor(o => o.AppEmailName).NotEmpty();
            RuleFor(o => o.AppEmailUser).NotEmpty();
            RuleFor(o => o.AppEmailPassword).NotEmpty();
            RuleFor(o => o.AppSMTPAddress).NotEmpty();
            RuleFor(o => o.AppSMTPPort).NotEmpty();
            RuleFor(o => o.AppEmailFrom).NotEmpty();
            RuleFor(o => o.AppEmailSubject).NotEmpty();
            RuleFor(o => o.AppCronPattern).NotEmpty();
            RuleFor(o => o.AppNotificationTriggerDelta).NotEmpty();
            RuleFor(o => o).Custom((o, context) =>
            {
                if (String.IsNullOrEmpty(o.AppEmailText) && String.IsNullOrEmpty(o.AppEmailHtml))
                {
                    context.AddFailure("Um dos campos a seguir deve ser preenchido: Texto do E-mail, Conteúdo HTML do e-mail");
                }
            });

        }
    }
}
