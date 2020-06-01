using Newtonsoft.Json;
using System.IO;

namespace BSMM2.Models.Matches.SingleMatch {

	internal class SingleMatchResult : IResult {

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

		[JsonIgnore]
		public string Information
			=> throw new System.NotImplementedException();

		public SingleMatchResult(RESULT_T result, int lifePoint = 0) {
			RESULT = result;
			LifePoint = lifePoint;
		}

		public void ExportTitle(TextWriter writer) {
			throw new System.NotImplementedException();
		}

		public void ExportData(TextWriter writer) {
			throw new System.NotImplementedException();
		}
	}
}