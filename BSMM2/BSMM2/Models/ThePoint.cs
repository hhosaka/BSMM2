using Newtonsoft.Json;

namespace BSMM2.Models {

	[JsonObject]
	internal class ThePoint : IPoint {

		[JsonIgnore]
		public int Point => _point.Point;

		[JsonIgnore]
		public int LifePoint => _point.LifePoint;

		[JsonIgnore]
		public double WinPoint => _point.WinPoint;

		[JsonProperty]
		private IPoint _point;

		public ThePoint(IResult result) {
			_point = result.GetPoint();
		}
	}
}