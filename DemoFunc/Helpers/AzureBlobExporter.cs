using Azure.Storage.Blobs;
using DemoFunc.Models;
using System;
using System.Threading.Tasks;

namespace DemoFunc.Helpers
{
    /// <summary>
    /// A basic Azure Blob exporter
    /// </summary>
    public static class AzureBlobExporter
    {
        private static readonly string azureConnectionString = "DefaultEndpointsProtocol=https;AccountName=anvutest;AccountKey=EvwtB8nOV7Mf4CcOuxPzq9ZFyQptCLjq/oCwE+PiXX+M3rw+P7GHCKyPiS26om16OCXnGP36pDIk+AStugLYcA==;EndpointSuffix=core.windows.net";
        private static readonly string containerName = "testing";

        public static async Task<FeedEtlResult> ExportAsync(TransformData data)
        {
            BlobContainerClient blobContainerClient = new(azureConnectionString, containerName);

            string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "-Orders.csv";
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            _ = await blobClient.UploadAsync(BinaryData.FromString(data.Contents), overwrite: true);

            return FeedEtlResult.Success;
        }

        public static async Task<TransformData> GetAsync()
        {
            BlobContainerClient blobContainerClient = new(azureConnectionString, containerName);

            BlobClient blobClient = blobContainerClient.GetBlobClient("test.csv");

            var response = await blobClient.DownloadContentAsync();
            return TransformData.Success(response.Value.Content.ToString());
        }
    }
}
