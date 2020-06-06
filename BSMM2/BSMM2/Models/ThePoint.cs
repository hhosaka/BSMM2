using Newtonsoft.Json;
using System.IO;

namespace BSMM2.Models {

	[JsonObject]
	internal class ThePoint : IPoint {

		private class TheResult : IResult {
			private IPoint _point;

			public int Point => _point.Point;

			public int LifePoint => _point.LifePoint;

			public double WinPoint => _point.WinPoint;

			public RESULT_T RESULT => RESULT_T.Progress;

			public bool IsFinished => true;

			public IPoint GetPoint() => _point;

			public string Information
				=> "Point = " + Point + " /Life = " + ToLifePoint(LifePoint) + " /Win = " + WinPoint;

			private string ToLifePoint(int lifePoint)
				=> lifePoint >= 0 ? lifePoint.ToString() : "-";

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

			public TheResult(IPoint point) {
				_point = point;
			}
		}

		[JsonIgnore]
		public int Point {
			get {
				switch (_result) {
					case RESULT_T.Win:
						return 3;

					case RESULT_T.Draw:
						return 1;

					default:
						return 0;
				}
			}
		}

		[JsonIgnore]
		public int LifePoint => _lifePoint;

		[JsonIgnore]
		public double WinPoint {
			get {
				switch (_result) {
					case RESULT_T.Win:
						return 1.0;

					case RESULT_T.Draw:
						return 0.5;

					default:
						return 0;
				}
			}
		}

		[JsonProperty]
		private RESULT_T _result;

		[JsonProperty]
		private int _lifePoint;

		public ThePoint(RESULT_T result, int lifePoint = 5) {
			_result = result;
			_lifePoint = lifePoint;
		}
	}
}