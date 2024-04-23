using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.RealTime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TrainMap.Data
{
	public class TrainDataSource : DynamicEntityDataSource
	{
		// Hold a reference to the file stream reader, the process task, and the cancellation token source.
		private Task? _processTask;
		private CancellationTokenSource? _cancellationTokenSource;

		public TimeSpan Delay { get; private set; }

		private TrainDataSource()
		{
			Delay = TimeSpan.FromSeconds(5);
		}

		public TrainDataSource(TimeSpan delay)
		{
			Delay = delay;
		}

		/// <summary>
		/// Initialize the dynamic data source.
		/// </summary>
		/// <returns></returns>
		protected override Task<DynamicEntityDataSourceInfo> OnLoadAsync()
		{
			// Create a new DynamicEntityDataSourceInfo using the entity ID field and the fields derived from the attributes of each observation in the custom data source.
			return Task.FromResult(new DynamicEntityDataSourceInfo("treinNummer", GetSchema())
			{
				SpatialReference = SpatialReferences.Wgs84 
			});
		}

		/// <summary>
		/// Connect to the dynamic data source.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected override Task OnConnectAsync(CancellationToken cancellationToken)
		{
			// On connecting to the custom data source begin processing the file. 
			_cancellationTokenSource = new CancellationTokenSource();
			_processTask = Task.Run(() => ObservationProcessLoopAsync(), _cancellationTokenSource.Token);
			return Task.CompletedTask;
		}

		/// <summary>
		/// Disconnect the dynamic data source
		/// </summary>
		/// <returns></returns>
		protected override async Task OnDisconnectAsync()
		{
			// On disconnecting from the custom data source, stop processing the file.
			_cancellationTokenSource?.Cancel();

			if (_processTask != null)
			{
				await _processTask;
			}

			_cancellationTokenSource?.Dispose();
			_cancellationTokenSource = null;
			_processTask = null;
		}

		/// <summary>
		/// Dynamic data source loop for getting the data. 
		/// </summary>
		/// <returns></returns>
		private async Task ObservationProcessLoopAsync()
		{
			try
			{
				while (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
				{
					// Process the next observation.
					bool processed = await ProcessNextObservation();

					// If the observation was not processed, continue to the next observation.
					if (!processed)
					{
						continue;
					}

					// If there is no delay, yield to the UI thread otherwise delay for the specified amount of time.
					if (Delay == TimeSpan.Zero)
					{
						await Task.Yield();
					}
					else
					{
						await Task.Delay(Delay, _cancellationTokenSource.Token);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// Read the next observation (train data)
		/// </summary>
		/// <returns></returns>
		private async Task<bool> ProcessNextObservation()
		{
			try
			{
				List<Treinen>? trains = await TrainLogic.GetLocations();
				if (trains == null)
				{
					return false;
				}

				foreach (Treinen train in trains)
				{
					// Add the observation to the custom data source.
					AddObservation(new MapPointBuilder(train.Lng, train.Lat, SpatialReferences.Wgs84).ToGeometry(), new Dictionary<string, object?>() {
						{ "treinNummer", train.TreinNummer.ToString() },
						{ "ritId", train.RitId},
						{ "snelheid", train.Snelheid},
						{ "richting", train.Richting},
						{ "horizontaleNauwkeurigheid", train.HorizontaleNauwkeurigheid},
						{ "type", train.Type},
						{ "bron", train.Bron},
					});
				}

				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"{ex}");
				return false;
			}
		}

		/// <summary>
		/// Data model of the dynamic data source
		/// </summary>
		/// <returns></returns>
		private static List<Field> GetSchema()
		{
			// Return a list of fields matching the attributes of each observation in the custom data source.
			return
			[
				new (FieldType.Text, "treinNummer", string.Empty, 8),
				new (FieldType.Text, "ritId", string.Empty, 10),
				new (FieldType.Float64, "snelheid", string.Empty, 8),
				new (FieldType.Float64, "richting", string.Empty, 8),
				new (FieldType.Float64, "horizontaleNauwkeurigheid", string.Empty, 8),
				new (FieldType.Text, "type", string.Empty, 10),
				new (FieldType.Text, "bron", string.Empty, 10),
			];
		}
	}
}
