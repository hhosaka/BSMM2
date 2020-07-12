using BSMM2.Resource;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BSMM2.Models {

	[JsonObject]
	internal class BYE : IPlayer {

		private class NullPoint : IExportablePoint {
			public string Information => throw new NotImplementedException();

			public int Value => -1;

			public int MatchPoint => -1;

			public double WinPoint => -1.0;

			public int? LifePoint => -1;

			public void ExportData(TextWriter writer) => throw new NotImplementedException();

			public void ExportTitle(TextWriter writer) => throw new NotImplementedException();
		}

		[JsonIgnore]
		private static IExportablePoint _nullPoint = new NullPoint();

		[JsonIgnore]
		public string Name => AppResources.TextBYE;

		[JsonIgnore]
		public bool Dropped => false;

		[JsonIgnore]
		public IExportablePoint Point => _nullPoint;

		[JsonIgnore]
		public IExportablePoint OpponentPoint => _nullPoint;

		[JsonIgnore]
		public bool HasByeMatch => true;

		[JsonIgnore]
		public bool HasGapMatch => true;

		public void Commit(Match match) {
		}

		public void ExportData(TextWriter writer) => throw new NotImplementedException();

		public void ExportTitle(TextWriter writer) => throw new NotImplementedException();
	}
}