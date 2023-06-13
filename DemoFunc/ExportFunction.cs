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
    public static class ExportFunction
    {
        [FunctionName("ExportFunction")]
        public static async Task Run([TimerTrigger("%TimerSchedule%")] TimerInfo myTimer, ILogger log) // 0 */5 * * * * => once every five minutes.
        {
            var result = await ExportCatalog(log);

            log.LogInformation($"Export {result.Status}.");
        }

        private static async Task<FeedEtlResult> ExportCatalog(ILogger log)
        {
            FeedEtlResult export;
            try
            {
                TransformerExporter.Initialize();

                await foreach (List<OrderDto> orderDtos in GetOrdersAsync())
                {
                    TransformerExporter.Transform(orderDtos);
                }

                log.LogInformation("Transforming complete, starting exports.");

                export = await TransformerExporter.CompleteAndExportAsync();
            }
            catch (Exception ex)
            {
                export = new FeedEtlResult(FeedETLStatus.FailureOnLoad, ex.Message);
            }

            return export;
        }

        private static IAsyncEnumerable<List<OrderDto>> GetOrdersAsync()
        {
            return GetAllDocumentsAsync(1000);
        }

        private static async IAsyncEnumerable<List<OrderDto>> GetAllDocumentsAsync(int pageSize)
        {
            int pageIndex = 0;

            List<OrderDto> orders = await AzureDataLayer.GetOrdersAsync(pageIndex, pageSize);

            yield return orders;

            while (orders.Any())
            {
                pageIndex++;
                orders = await AzureDataLayer.GetOrdersAsync(pageIndex, pageSize);

                yield return orders;
            }
        }
    }
}
