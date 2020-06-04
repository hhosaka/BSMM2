using Newtonsoft.Json;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	internal class MultiMatch : Match {

		[JsonProperty]
		private MultiMatchRule _rule;

		public MultiMatch() {
		}

		public MultiMatch(MultiMatchRule rule, IPlayer player1, IPlayer player2)
			: base(rule, player1, player2) {
			_rule = rule;
		}

		public override void SetResult(RESULT_T result) {
			SetResults(_rule.CreatePoints(result));
		}
	}
}