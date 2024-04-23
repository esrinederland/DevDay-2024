using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrainMap.Data
{
	public class Payload
	{
		[JsonProperty("treinen")]
		public List<Treinen>? Treinen { get; set; }
	}

	public class TrainResponse
	{
		[JsonProperty("payload")]
		public Payload? Payload { get; set; }
	}

	public class Treinen
	{
		[JsonProperty("treinNummer")]
		public int? TreinNummer { get; set; }

		[JsonProperty("ritId")]
		public string? RitId { get; set; }

		[JsonProperty("lat")]
		public double Lat { get; set; }

		[JsonProperty("lng")]
		public double Lng { get; set; }

		[JsonProperty("snelheid")]
		public double? Snelheid { get; set; }

		[JsonProperty("richting")]
		public double? Richting { get; set; }

		[JsonProperty("horizontaleNauwkeurigheid")]
		public double? HorizontaleNauwkeurigheid { get; set; }

		[JsonProperty("type")]
		public string? Type { get; set; }

		[JsonProperty("bron")]
		public string? Bron { get; set; }
	}
}