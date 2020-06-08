﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSMM2.Models.Matches.SingleMatch {

	internal class SingleMatchResult : IResult {

		private class TheResult : IExportablePoint {
			public int Point { get; }

			public int LifePoint { get; }

			public double WinPoint { get; }

			public string Information
				=> "Point = " + Point + " /Life = " + (LifePoint >= 0 ? LifePoint.ToString() : "-") + " /Win = " + WinPoint;

			public void ExportTitle(TextWriter writer) {
				writer.Write("Point, WinPoint, LifePoint");
			}

			public void ExportData(TextWriter writer) {
				writer.Write(Point);
				writer.Write(", ");
				writer.Write(WinPoint);
				writer.Write(", ");
				writer.Write(LifePoint);
			}

			public int Value => Point;

			public TheResult() {
			}

			public TheResult(IEnumerable<IPoint> source) {
				foreach (var point in source) {
					if (point != null) {
						Point += point.Point;
						LifePoint += point.LifePoint;
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
		public int Point
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