using Microsoft.Extensions.Logging;
using Azure.Storage.Queues;
using CreateWebMaps.Logic.Settings;

namespace CreateWebMaps.Logic
{
    public static class QueueLogic
    {
        public static async Task<QueueClient?> GetOrCreateQueue(ILogger log, string queueName)
        {
            // Get the storage url.
            string? azureStoragePath = Environment.GetEnvironmentVariable(Constants.AzureWebJobsStorage);
            if (azureStoragePath == null)
            {
                log.LogError("Azure storage account not set.");
                return null;
            }

            // Encode the queue messages to base64
            QueueClientOptions queueOptions = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            // Get the storage account with the path.
            QueueClient queueClient = new QueueClient(azureStoragePath, queueName.ToLower().Replace("_", string.Empty), queueOptions);
        
            // Create the queue if it does not exist.
            await queueClient.CreateIfNotExistsAsync();

            // Check if the queue is available.
            if (await queueClient.ExistsAsync())
            {
                return queueClient;
            }
            else
            {
                // Log message
                log.LogError($"Queue not found or can't create a queue for {queueName.ToLower().Replace("_", string.Empty)}.");
                return null;
            }
        }
    }
}
