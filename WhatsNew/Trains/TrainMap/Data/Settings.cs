using Newtonsoft.Json;
using System.Drawing;

namespace TrainMap.Data
{
	internal static class Settings
	{
		internal const string ServiceUrl = @"https://gateway.apiportal.ns.nl/virtual-train-api/api";

		internal const string ServiceKey = "NS api key";

		internal const string RailLayerPath = @"D:\Data\SpoortakMobile.geodatabase";

		internal const string RailLayerUrl = @"https://mapservices.prorail.nl/arcgis/rest/services/Geleidingssysteem_010/FeatureServer/39";

		internal static readonly Color RailColor = Color.FromArgb(179, 139, 127);

		internal static readonly Color YellowColor = Color.FromArgb(0, 99, 211);

		internal static readonly Color BlueColor = Color.FromArgb(255, 201, 23);

		internal static readonly Color RedColor = Color.FromArgb(201, 7, 16);
	}
}
