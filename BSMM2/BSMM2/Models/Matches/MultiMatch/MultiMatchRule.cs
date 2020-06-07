using Newtonsoft.Json;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	public abstract class MultiMatchRule : Rule {
		public int MatchCount { get; }
		public int MinimumMatchCount { get; }

		protected MultiMatchRule(int matchCount, int minimumMatchCount) {
			MatchCount = matchCount;
			MinimumMatchCount = minimumMatchCount;
		}
	}
}