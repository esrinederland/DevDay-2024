using CreateWebMaps.Logic;
using EsriNL.Net.API.Data.FeatureService;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CreateWebMaps
{
    public class CreateWebMapsQueueListener
    {
        private readonly ILogger<CreateWebMapsQueueListener> _logger;

        public CreateWebMapsQueueListener(ILogger<CreateWebMapsQueueListener> logger)
        {
            _logger = logger;
        }

        [Function(nameof(CreateWebMapsQueueListener))]
        public void Run([QueueTrigger("%CreateWebMapsQueueName%", Connection = "AzureWebJobsStorage")] string queuePayload)
        {
            if (string.IsNullOrWhiteSpace(queuePayload))
            {
                _logger.LogWarning("Queue message is empty.");
                return;
            }

            List<WebhookPayload>? data = null;
            if (queuePayload.StartsWith('['))
            {
                // Convert the request body to a .net object. 
                data = WebhookPayload.Creates(queuePayload);
            }
            else
            {
                data = new List<WebhookPayload>() { WebhookPayload.Create(queuePayload) };
            }

            if (data == null)
            {
                _logger.LogWarning("Can't convert queue message to webhookpayload.");
                return;
            }

            foreach (WebhookPayload payload in data)
            {
                if (WebMapsLogic.Create(_logger, payload))
                {
                    _logger.LogInformation("Succesfully created webmaps.");
                }
                else
                {
                    _logger.LogWarning("Failed to created webmaps.");
                }
            }
        }
    }
}
