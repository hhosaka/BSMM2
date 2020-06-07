using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	public abstract class MultiMatchRule : SingleMatchRule {
		public int MatchCount { get; }
		public int MinimumMatchCount { get; }

		protected MultiMatchRule(int matchCount, int minimumMatchCount) {
			MatchCount = matchCount;
			MinimumMatchCount = minimumMatchCount;
		}
	}
}