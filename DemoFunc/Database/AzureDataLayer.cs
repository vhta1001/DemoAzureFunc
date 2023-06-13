using DemoFunc.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoFunc.Database
{
    public static class AzureDataLayer
    {
        public static string ConnectionString { get; } = "Server=tcp:anvusql.database.windows.net,1433;Initial Catalog=databasetest;Persist Security Info=False;User ID=dev-sqlaccess;Password=Sysnify5355;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public static DatabaseCommander Commander { get; } = new DatabaseCommander(ConnectionString);

        public static async Task<List<OrderDto>> GetOrdersAsync(int pageIndex, int pageSize)
        {
            using DatabaseLiveCommander commander = new(ConnectionString, 120);

            string sql = @"SELECT [OrderId],
                                    [OrderStatus],
                                    [TimePlaced],
                                    [TotalAmount]
                                    FROM [dbo].[Orders]
                                    ORDER BY TimePlaced
                                    OFFSET @pageIndex*@pageSize ROWS
                                    FETCH NEXT @pageSize ROWS ONLY;";

            var orders = await commander.QueryListAsync(sql, (dr) => new OrderDto
            {
                OrderId = dr.GetGuid(0),
                OrderStatus = dr.GetInt32(1),
                TimePlaced = dr.GetDateTime(2),
                TotalAmount = dr.GetDecimal(3),
            },
                    new SqlParameter("pageIndex", pageIndex),
                    new SqlParameter("pageSize", pageSize));

            return orders;
        }
    }
}
