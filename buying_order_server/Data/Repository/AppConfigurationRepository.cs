using buying_order_server.Contracts;
using buying_order_server.Data.Entity;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace buying_order_server.Data.Repository
{
    public class AppConfigurationRepository : DbFactoryBase, IAppConfigurationRepository
    {
        private readonly ILogger<AppConfigurationRepository> _logger;

        public AppConfigurationRepository(IConfiguration config, ILogger<AppConfigurationRepository> logger) : base(config)
        {
            _logger = logger;
        }


        public async Task<string> GetCronPattern()
        {
            var query = @"SELECT ""AppCronPattern"" FROM ""AppConfiguration""
                            ORDER BY ""Id"" DESC 
                            LIMIT 1";

            return await DbQuerySingleAsync<string>(query);
        }

        public async Task<AppConfiguration> GetLastAsync()
        {
            var query = @"SELECT * FROM ""AppConfiguration""
                            ORDER BY ""Id"" DESC 
                            LIMIT 1";

            return await DbQuerySingleAsync<AppConfiguration>(query);
        }

        public async Task<IEnumerable<AppConfiguration>> GetAllAsync()
        {
            var query = @"SELECT * FROM ""AppConfiguration""
                            ORDER BY ""Id"" DESC";

            var data = await DbQueryAsync<AppConfiguration>(query);
            return data;
        }

        public async Task<bool> CreateAsync(AppConfiguration entity)
        {
            var stmt = @"INSERT INTO ""AppConfiguration"" 
                            (""AppBlacklist"", ""AppCronPattern"", ""AppCronTimezone"", ""AppEmailFrom"",
                            ""AppEmailHtml"", ""AppEmailName"", ""AppEmailPassword"", ""AppEmailSubject"",
                            ""AppEmailText"", ""AppEmailUser"", ""AppNotificationTriggerDelta"",
                            ""AppSMTPPort"", ""AppSMTPSecure"", ""AppSMTPAddress"") 
                        VALUES 
                            (@AppBlacklist, @AppCronPattern, @AppCronTimezone, @AppEmailFrom,
                            @AppEmailHtml, @AppEmailName, @AppEmailPassword, @AppEmailSubject,
                            @AppEmailText, @AppEmailUser, @AppNotificationTriggerDelta,
                            @AppSMTPPort, @AppSMTPSecure, @AppSMTPAddress)";

            var parameters = new DynamicParameters();
            parameters.Add("AppBlacklist", entity.AppBlacklist);
            parameters.Add("AppCronPattern", entity.AppCronPattern);
            parameters.Add("AppCronTimezone", entity.AppCronTimezone);
            parameters.Add("AppEmailFrom", entity.AppEmailFrom);
            parameters.Add("AppEmailHtml", entity.AppEmailHtml);
            parameters.Add("AppEmailName", entity.AppEmailName);
            parameters.Add("AppEmailPassword", entity.AppEmailPassword);
            parameters.Add("AppEmailSubject", entity.AppEmailSubject);
            parameters.Add("AppEmailText", entity.AppEmailText);
            parameters.Add("AppEmailUser", entity.AppEmailUser);
            parameters.Add("AppNotificationTriggerDelta", entity.AppNotificationTriggerDelta);
            parameters.Add("AppSMTPPort", entity.AppSMTPPort);
            parameters.Add("AppSMTPSecure", entity.AppSMTPSecure);
            parameters.Add("AppSMTPAddress", entity.AppSMTPAddress);

            return await DbExecuteAsync<AppConfiguration>(stmt, parameters);
        }

        public Task<AppConfiguration> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(AppConfiguration entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<(IEnumerable<OrderNotification> Orders, Pagination Pagination)> paginatedExampleGetAppConfigurationAsync(UrlQueryParameters urlQueryParameters)
        {
            IEnumerable<OrderNotification> persons;
            int recordCount = default;

            var query = @"SELECT * FROM ""OrderNotification""
                            ORDER BY ID DESC 
                            Limit @Limit Offset @Offset";


            var param = new DynamicParameters();
            param.Add("Limit", urlQueryParameters.PageSize);
            param.Add("Offset", urlQueryParameters.PageNumber);

            if (urlQueryParameters.IncludeCount)
            {
                query += " SELECT COUNT(ID) FROM \"OrderNotification\"";
                var pagedRows = await DbQueryMultipleAsync<OrderNotification, int>(query, param);

                persons = pagedRows.Data;
                recordCount = pagedRows.RecordCount;
            }
            else
            {
                persons = await DbQueryAsync<OrderNotification>(query, param);
            }

            var metadata = new Pagination
            {
                PageNumber = urlQueryParameters.PageNumber,
                PageSize = urlQueryParameters.PageSize,
                TotalRecords = recordCount

            };

            return (persons, metadata);

        }

        public async Task<bool> ExecuteWithTransactionScope()
        {

            using (var dbCon = new SqlConnection(DbConnectionString))
            {
                await dbCon.OpenAsync();
                var transaction = await dbCon.BeginTransactionAsync();

                try
                {
                    //Do stuff here Insert, Update or Delete
                    Task q1 = dbCon.ExecuteAsync("<Your SQL Query here>");
                    Task q2 = dbCon.ExecuteAsync("<Your SQL Query here>");
                    Task q3 = dbCon.ExecuteAsync("<Your SQL Query here>");

                    await Task.WhenAll(q1, q2, q3);

                    //Commit the Transaction when all query are executed successfully

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    //Rollback the Transaction when any query fails
                    transaction.Rollback();
                    _logger.Log(LogLevel.Error, ex, "Error when trying to execute database operations within a scope.");

                    return false;
                }
            }
            return true;
        }

    }
}
