using BSMM2.Resource;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSMM2.Models.Matches.SingleMatch {

	internal class SingleMatchResult : IResult, IBSPoint {

		private class TheResult : IExportablePoint, IBSPoint {
			public int MatchPoint { get; }

			public int LifePoint { get; }

			public double WinPoint { get; }

			public string Information
				=> AppResources.TextMatchPoint + " = " + MatchPoint + " /" +
					AppResources.TextLifePoint + " = " + (LifePoint >= 0 ? LifePoint.ToString() : "-") + " /" +
					AppResources.TextWinPoint + " = " + WinPoint;

			public void ExportTitle(TextWriter writer) {
				writer.Write("Point, WinPoint, LifePoint");
			}

			public void ExportData(TextWriter writer) {
				writer.Write(MatchPoint);
				writer.Write(", ");
				writer.Write(WinPoint);
				writer.Write(", ");
				writer.Write(LifePoint);
			}

			public int Value => MatchPoint;

			public TheResult() {
			}

			public TheResult(IEnumerable<IPoint> source) {
				foreach (var point in source) {
					if (point != null) {
						MatchPoint += point.MatchPoint;
						LifePoint += (point as IBSPoint)?.LifePoint ?? -1;
						WinPoint += point.WinPoint;
					}
				}
				WinPoint = source.Any() ? WinPoint / source.Count() : 0.0;
			}
		}

		public static IExportablePoint Total(IEnumerable<IPoint> points) {
			return new TheResult(points);
		}

		[JsonIgnore]
		public int MatchPoint
			=> RESULT == Models.RESULT_T.Win ? 3 : RESULT == Models.RESULT_T.Lose ? 0 : 1;

		[JsonIgnore]
		public double WinPoint
			=> RESULT == Models.RESULT_T.Win ? 1.0 : RESULT == Models.RESULT_T.Lose ? 0.0 : 0.5;

		[JsonProperty]
		public int LifePoint { get; }

		[JsonProperty]
		public RESULT_T RESULT { get; }

		[JsonIgnore]
		public bool IsFinished => RESULT != RESULT_T.Progress && LifePoint >= 0;

		public SingleMatchResult(RESULT_T result, int lifePoint = 0) {
			RESULT = result;
			LifePoint = lifePoint;
		}
	}
}