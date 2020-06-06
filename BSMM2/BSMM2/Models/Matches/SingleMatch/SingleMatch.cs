using Newtonsoft.Json;

namespace BSMM2.Models.Matches.SingleMatch {

	[JsonObject]
	public class SingleMatch : Match {

		[JsonProperty]
		private SingleMatchRule Rule => _rule as SingleMatchRule;

		[JsonProperty]
		private IScore _score;

		public SingleMatch() {
		}

		public SingleMatch(SingleMatchRule rule, IPlayer player1, IPlayer player2)
			: base(rule, player1, player2) {
		}

		public override void SetResult(RESULT_T result) {
			_score = new Score(result);
			SetResults(Rule.CreatePoint(new Score(result, 5, 5)));
		}

		public void SetResult(RESULT_T result, int lp1, int lp2) {
			_score = new Score(result, lp1, lp2);
			SetResults(Rule.CreatePoint(new Score(result, lp1, lp2)));
		}
	}
}