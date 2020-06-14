using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	public abstract class MultiMatchRule : SingleMatchRule {
		public abstract int MatchCount { get; }
		public abstract int MinimumMatchCount { get; }

		protected MultiMatchRule() {
		}

		protected MultiMatchRule(Rule rule) : base(rule) {
		}
	}
}