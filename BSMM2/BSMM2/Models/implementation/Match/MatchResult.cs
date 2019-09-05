using Newtonsoft.Json;

namespace BSMM2.Models.Rules.Match {

	internal class MatchResult : IMatchResult {

		[JsonIgnore]
		public int MatchPoint
			=> RESULT == Models.RESULT.Win ? 3 : RESULT == Models.RESULT.Lose ? 0 : 1;

		[JsonIgnore]
		public double WinPoint
			=> RESULT == Models.RESULT.Win ? 1.0 : RESULT == Models.RESULT.Lose ? 0.0 : 0.5;

		[JsonProperty]
		public int LifePoint { get; }

		[JsonProperty]
		public RESULT? RESULT { get; }

		[JsonIgnore]
		public int Point => MatchPoint;

		public bool IsFinished => true;

		public MatchResult(RESULT result, int lifePoint = 0) {
			RESULT = result;
			LifePoint = lifePoint;
		}
	}
}