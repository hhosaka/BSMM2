﻿using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	public class MultiMatch : SingleMatch.SingleMatch {

		public class Score {
			public RESULT_T Result { get; }
			public int LifePoint1 { get; }
			public int LifePoint2 { get; }

			public Score(RESULT_T result, int lp1 = 0, int lp2 = 0) {
				Result = result;
				LifePoint1 = lp1;
				LifePoint2 = lp2;
			}
		}

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
				scores.Add(new Score(result));
			}
			SetMultiMatchResult(scores);
		}

		public void SetMultiMatchResult(IEnumerable<Score> scores) {
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