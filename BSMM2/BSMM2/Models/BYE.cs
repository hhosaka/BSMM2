using BSMM2.Resource;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BSMM2.Models {

	[JsonObject]
	internal class BYE : IPlayer {

		private class NullPoint : IExportablePoint {
			public string Information => throw new NotImplementedException();

			public int MatchPoint => 0;

			public double WinPoint => 0.0;

			public int? LifePoint => 0;

			public void ExportData(TextWriter writer) => throw new NotImplementedException();

			public void ExportTitle(TextWriter writer) => throw new NotImplementedException();
		}

		[JsonIgnore]
		private static IExportablePoint _nullPoint = new NullPoint();

		[JsonIgnore]
		public string Name => AppResources.TextBYE;

		[JsonIgnore]
		public IExportablePoint Point => _nullPoint;

		[JsonIgnore]
		public IExportablePoint OpponentPoint => _nullPoint;

		public void ExportData(TextWriter writer) => throw new NotImplementedException();

		public void ExportTitle(TextWriter writer) => throw new NotImplementedException();
	}
}