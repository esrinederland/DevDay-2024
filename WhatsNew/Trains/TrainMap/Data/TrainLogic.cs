using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrainMap.Data
{
	public static class TrainLogic
	{
		/// <summary>
		/// Call's the NS Train service for all the trains, returns a json with the current running trains and location.
		/// When the response is correct, the json converter creates an list of train objects.
		/// </summary>
		/// <returns></returns>
		public static async Task<List<Treinen>?> GetLocations()
		{
			using RestClient client = new(Settings.ServiceUrl);

			client.AddDefaultHeader("Ocp-Apim-Subscription-Key", Settings.ServiceKey);

			RestRequest restRequest = new("vehicle");

			RestResponse restResponse = await client.ExecuteAsync(restRequest);

			if (restResponse?.Content != null && restResponse.IsSuccessful)
			{
				TrainResponse? trainResponse = JsonConvert.DeserializeObject<TrainResponse>(restResponse.Content);

				return trainResponse?.Payload?.Treinen;
			}

			return null;
		}
	}
}
