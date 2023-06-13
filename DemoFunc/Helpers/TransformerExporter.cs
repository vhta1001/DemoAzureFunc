using DemoFunc.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoFunc.Helpers
{
    /// <summary>
    /// Combines a Transformer and Exporter together with the configuration and settings
    /// </summary>
    public static class TransformerExporter
    {
        public static TransformData Complete()
        {
            return CsvTransformer.Complete();
        }

        public static async Task<FeedEtlResult> CompleteAndExportAsync()
        {
            try
            {
                TransformData data;
                try
                {
                    data = CsvTransformer.Complete();
                }
                catch (Exception e)
                {
                    return FeedEtlResult.FailureOnTransform(e);
                }

                return await AzureBlobExporter.ExportAsync(data);
            }
            catch (Exception e)
            {
                return FeedEtlResult.FailureOnExport(e);
            }
        }

        public static Task<TransformData> GetAsync()
        {
            return AzureBlobExporter.GetAsync();
        }

        public static void Initialize()
        {
            CsvTransformer.Initialize();
        }

        public static void Transform(List<OrderDto> orders)
        {
            CsvTransformer.Transform(orders);
        }
    }
}
