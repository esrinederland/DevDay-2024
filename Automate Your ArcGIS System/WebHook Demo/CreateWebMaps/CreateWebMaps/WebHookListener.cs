using Azure.Storage.Queues;
using CreateWebMaps.Logic;
using EsriNL.Net.API.Data.FeatureService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CreateWebMaps
{
    public class WebHookListener
    {
        private readonly ILogger<WebHookListener> _logger;

        public WebHookListener(ILogger<WebHookListener> logger)
        {
            _logger = logger;
        }

        [Function("WebHookListener")]
        public async Task<IActionResult> Run([HttpTrigger(Microsoft.Azure.Functions.Worker.AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            // Startup logs and read the request.
            _logger.LogInformation("Start webhook function.");
            string requestBody = await req.ReadAsStringAsync();
            _logger.LogInformation($"Webhook body: {requestBody}");

            if (requestBody.StartsWith("payload"))
            {
                _logger.LogError("Edit your webhook, use Content Type -> application/json");
            }

            List<WebhookPayload>? data = null;
            if (requestBody.StartsWith("["))
            {
                // Convert the request body to a .net object. 
                data = WebhookPayload.Creates(requestBody);
            }
            else
            {
                data = new List<WebhookPayload>() { WebhookPayload.Create(requestBody) };
            }

            // Null and data checks. 
            if (data != null && data.Count != 0)
            {
                // For every data element get the payload and read the servicename
                foreach (string serviceName in data.Select(payload => payload.ServiceName))
                {
                    if (!string.IsNullOrEmpty(serviceName))
                    {
                        _logger.LogInformation($"Try to get or create queue {serviceName}");

                        // Get or create the queue based on the service name.
                        QueueClient? queue = await QueueLogic.GetOrCreateQueue(_logger, serviceName);

                        // Check if the queue is available.
                        if (queue != null)
                        {
                            // Log message
                            _logger.LogInformation("Queue find, set the notify item on the queue.");

                            // Create a message and add it to the queue.
                            await queue.SendMessageAsync(requestBody);

                            // Log message
                            _logger.LogInformation("NotifyItem set succesfully on the queue.");

                            // Send item succes response
                            return new OkObjectResult("Item in progress.");
                        }
                        else
                        {
                            // Write a log warning in case the script can't create the queue
                            _logger.LogWarning($"Queue not found or unable to create the queue {serviceName}");
                        }
                    }
                    else
                    {
                        // Write a log warning in case the script can't create the queue
                        _logger.LogWarning($"Service name is empty, can't create a queue.");
                    }
                }
            }
            else
            {
                // No data 
                return new EmptyResult();
            }

            // Function failed.
            return new BadRequestResult();
        }
    }
}
