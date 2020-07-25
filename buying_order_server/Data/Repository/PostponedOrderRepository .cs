using buying_order_server.Contracts;
using buying_order_server.Data.Entity;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace buying_order_server.Data.Repository
{
    public class PostponedOrderRepository : DbFactoryBase, IPostponedOrderRepository
    {
        private readonly ILogger<PostponedOrderRepository> _logger;

        public PostponedOrderRepository(IConfiguration config, ILogger<PostponedOrderRepository> logger) : base(config)
        {
            _logger = logger;
        }

        public async Task<PostponedOrderEntity> CreateOrUpdateAsync(PostponedOrderEntity entity)
        {
            var stmt = @"INSERT INTO ""PostponedOrder"" as PO 
                            (""OrderId"", ""Date"", ""Count"") 
                        VALUES 
                            (@OrderId, @Date, @Count)
                        ON CONFLICT (""OrderId"") DO
                        UPDATE
	                        SET ""Date"" = @Date, ""Count"" = PO.""Count"" + 1";

            var parameters = new DynamicParameters();
            parameters.Add("OrderId", entity.OrderId);
            parameters.Add("Date", entity.Date);
            parameters.Add("Count", 1);

            await DbExecuteAsync<PostponedOrderEntity>(stmt, parameters);
            return entity;
        }

        public async Task<PostponedOrderEntity> GetByIdAsync(object id)
        {
            var query = @"SELECT * FROM ""PostponedOrder""
                          WHERE ""OrderId"" = @OrderId 
                          LIMIT 1";

            var parameters = new DynamicParameters();
            var parsedId = Int32.Parse($"{id}");
            parameters.Add("OrderId", parsedId);

            return await DbQuerySingleAsync<PostponedOrderEntity>(query, parameters);
        }

        public Task<bool> DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistAsync(object id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<PostponedOrderEntity>> IRepository<PostponedOrderEntity>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<PostponedOrderEntity> IRepository<PostponedOrderEntity>.GetLastAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(PostponedOrderEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(PostponedOrderEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
