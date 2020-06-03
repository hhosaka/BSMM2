using Newtonsoft.Json;

namespace BSMM2.Models {

	[JsonObject]
	internal class ThePoint : IPoint {

		[JsonIgnore]
		public int Point => _result.Point;

		[JsonIgnore]
		public int LifePoint => _result.LifePoint;

		[JsonIgnore]
		public double WinPoint => _result.WinPoint;

		[JsonProperty]
		private IResult _result;

		private ThePoint(IResult result) {
			_result = result;
		}
	}
}