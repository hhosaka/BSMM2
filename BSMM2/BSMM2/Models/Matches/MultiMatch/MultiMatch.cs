using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	public class MultiMatch : SingleMatch.SingleMatch {

		[JsonProperty]
		private MultiMatchRule Rule => _rule as MultiMatchRule;

		public MultiMatch() {
		}

		public MultiMatch(MultiMatchRule rule, IPlayer player1, IPlayer player2)
			: base(rule, player1, player2) {
		}

		public override void SetResult(RESULT_T result) {
			var scores = new List<Score>();
			for (int i = 0; i < Rule.MatchCount; ++i) {
				scores.Add(new Models.Score(result));
			}
			SetMultiMatchResult(scores);
		}

		public void SetMultiMatchResult(IEnumerable<IScore> scores) {
			var result1 = new MultiMatchResult(Rule.MinimumMatchCount);
			var result2 = new MultiMatchResult(Rule.MinimumMatchCount);
			foreach (var score in scores) {
				result1.Add(new SingleMatchResult(score.Result, score.LifePoint1));
				result2.Add(new SingleMatchResult(RESULTUtil.ToOpponents(score.Result), score.LifePoint2));
			}
			SetResults(result1, result2);
		}
	}
}