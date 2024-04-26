using EsriNL.Net.API.Data;
using EsriNL.Net.API.Data.Content;
using EsriNL.Net.API.Data.FeatureService;
using Microsoft.Extensions.Logging;
using CreateWebMaps.Logic.Settings;
using Newtonsoft.Json;
using EsriNL.Net.API;
using System.Security.Cryptography;

namespace CreateWebMaps.Logic
{
    public static class WebMapsLogic
    {
        public static bool Create(ILogger log, WebhookPayload payload)
        {
            if (payload == null)
            {
                return false;
            }

            log.LogInformation("Start creating webmaps.");
            log.LogInformation("{className}::{methodName}::Start", nameof(WebMapsLogic), nameof(Create));

            // get the allready created arcgis client
            IArcGISClient client = ArcGISLogic.GetClient();
            if (client == null)
            {
                log.LogError("Failed to create a ArcGIS client.");
                return false;
            }

            if (payload.FeatureServiceUrl == null)
            {
                log.LogError("FeatureService Url is empty");
                return false;
            }

            log.LogInformation("{className}::{methodName}:: Create a ArcGIS .NET FeatureService", nameof(WebMapsLogic), nameof(Create));
            // Create a ArcGIS .NET FeatureService
            FeatureService jobstatusFeatureService = new (client, payload.FeatureServiceUrl);

            log.LogInformation("{className}::{methodName}::Get the changes", nameof(WebMapsLogic), nameof(Create));
            //get the changes from de given url from the webhook
            ChangesResponse changes = jobstatusFeatureService.GetChanges(payload.ChangesUrl);
            if (changes == null)
            {
                log.LogWarning("Can't extract changes from webhook.");
                return false;
            }

            log.LogInformation("{className}::{methodName}::Walk through all Edits", nameof(WebMapsLogic), nameof(Create));
            //loop through all Edits which are recieved from the change url which are send by the webhook 
            foreach (Edit edit in changes.Edits)
            {
                if (edit?.Features?.Adds != null && edit.Features.Adds.Count != 0)
                {
                    //loop through all new cerated feature off an edit
                    foreach (Feature feature in edit.Features.Adds)
                    {
                        if(!CreateWebMap(client, log, feature))                      
                        {
                            log.LogError("Failed to create a Map.");
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private static bool CreateWebMap(IArcGISClient client, ILogger log, Feature feature)
        {
            try
            {

                log.LogInformation("{className}::{methodName}::Start", nameof(WebMapsLogic), nameof(CreateWebMap));
                string? mapId = Environment.GetEnvironmentVariable(Constants.TemplateWebMapId);
                EsriNL.Net.API.Content.Item webmap = new(client, mapId);
                ItemInfoResponse webmapInfo = webmap.Info();
                ItemDataResponse webmapData = webmap.Data();

                string folderid = webmapInfo.OwnerFolder;

                string name = feature.GetAttribute<string>("name");
                string comment = feature.GetAttribute<string>("comment");

                var oid = feature.Attributes["objectid"];
                webmapData.OperationalLayers[0].LayerDefinition.DefinitionExpression = $"ObjectID={oid}";


                webmapInfo.Title = $"Generated webmap for {name}";
                webmapInfo.Tags = new List<string>() { "ESRI_Connect_24", "REST", "AutomateYourPlatform", "Demo", { name } };
                webmapInfo.Description = $"Hello {name}, this is a newly created webmap. You said: {comment}";
                webmapInfo.Type = "Web Map";
                webmapInfo.Text = JsonConvert.SerializeObject(webmapData);

                AddItemResponse addResult = webmap.Add(Environment.GetEnvironmentVariable(Constants.ArcGISUserName), webmapInfo, folderid);

                if (addResult == null)
                {
                    return false;
                }
                return addResult.Success;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to create a webmap.");
                return false;
            }
        }
        
    }
}

