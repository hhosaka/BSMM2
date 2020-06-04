using Newtonsoft.Json;

namespace BSMM2.Models.Matches.SingleMatch {

	[JsonObject]
	public class SingleMatch : Match {

		[JsonProperty]
		private SingleMatchRule _rule;

		public SingleMatch() {
		}

		public SingleMatch(SingleMatchRule rule, IPlayer player1, IPlayer player2)
			: base(rule, player1, player2) {
			_rule = rule;
		}

		public override void SetResult(RESULT_T result) {
			SetResults(_rule.CreatePoint(new Score(result, 5, 5)));
		}

		public void SetResult(RESULT_T result, int lp1, int lp2) {
			SetResults(_rule.CreatePoint(new Score(result, lp1, lp2)));
		}
	}
}