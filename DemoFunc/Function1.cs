using DemoFunc.Database;
using DemoFunc.Helpers;
using DemoFunc.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoFunc
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            int i = 0;

            var a = ExportCatalog(log);

            log.LogInformation($"Export {a.Status}.");
        }

        private async Task<FeedETLResult> ExportCatalog(ILogger log)
        {
            FeedETLResult allExports;
            try
            {
                TransformerExporter.Initialize();

                await foreach (List<OrderDto> orderDtos in GetOrdersAsync())
                {
                    TransformerExporter.Transform(orderDtos);
                }

                log.LogInformation("Transforming complete, starting exports.");

                allExports = await TransformerExporter.CompleteAndExportAsync();
            }
            catch (Exception ex)
            {
                allExports = new FeedETLResult(FeedETLStatus.FailureOnLoad, ex.Message);
            }

            return allExports;
        }

        public IAsyncEnumerable<List<OrderDto>> GetOrdersAsync()
        {
            return GetAllDocumentsAsync(1000);
        }

        public async IAsyncEnumerable<List<OrderDto>> GetAllDocumentsAsync(int pageSize)
        {
            int pageIndex = 0;

            List<OrderDto> orders = await AzureDataLayer.GetOrdersAsync(pageIndex, pageSize);

            yield return orders;

            while (orders.Any())
            {
                pageIndex++;

                orders = await GetOrderDtos(pageSize, pageIndex);

                yield return orders;
            }
        }

        public Task<List<OrderDto>> GetOrderDtos(int pageSize, int pageIndex)
        {
            List<OrderDto> orderDtos = new List<OrderDto>();

            if (pageIndex > 5)
            {
                return Task.FromResult(orderDtos);
            }

            for (int i = 0; i < pageSize; i++)
            {
                orderDtos.Add(new OrderDto()
                {
                    OrderId = Guid.NewGuid(),
                    OrderStatus = 1,
                    TimePlaced = DateTime.Now,
                });
            }

            return Task.FromResult(orderDtos);
        }
    }
}
