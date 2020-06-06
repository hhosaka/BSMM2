﻿using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static BSMM2.Models.RESULT_T;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	public abstract class MultiMatchRule : Rule {
		protected abstract int MatchCount { get; }
		protected abstract int MinimumMatchCount { get; }

		public IEnumerable<IScore> CreatePointsTentative(RESULT_T result) {
			var results = new List<IScore>();
			for (int i = 0; i < MatchCount; ++i) {
				results.Add(new Score(result));
			}
			return results;
		}

		public override IPoint CreatePoint(RESULT_T result)
			=> new ThePoint(result);

		public (IResult, IResult) CreatePoints(RESULT_T result) {
			var results = new List<RESULT_T>();
			for (int i = 0; i < MatchCount; ++i) {
				results.Add(result);
			}
			return CreatePoints(results.ToArray());
		}

		public (IResult, IResult) CreatePoint(IEnumerable<IScore> scores)
			=> CreatePoints(scores.Select(score => (score.Result, score.LifePoint1, score.LifePoint2)));

		public (IResult, IResult) CreatePoints(IEnumerable<RESULT_T> player1Results) {
			return CreatePoints(player1Results.Select(r => (r, 5, 5)));
		}

		public (IResult, IResult) CreatePoints(IEnumerable<(RESULT_T, int lp1, int lp2)> player1Results) {
			var p1result = new MultiMatchResult(MinimumMatchCount);
			var p2result = new MultiMatchResult(MinimumMatchCount);
			foreach (var result in player1Results) {
				switch (result.Item1) {
					case Win:
						p1result.Add(new SingleMatchResult(Win, result.lp1));
						p2result.Add(new SingleMatchResult(Lose, result.lp2));
						break;

					case Lose:
						p1result.Add(new SingleMatchResult(Lose, result.lp1));
						p2result.Add(new SingleMatchResult(Win, result.lp2));
						break;

					case Draw:
						p1result.Add(new SingleMatchResult(Draw, result.lp1));
						p2result.Add(new SingleMatchResult(Draw, result.lp2));
						break;

					default:
						throw new ArgumentException();
				}
			}
			return (p1result, p2result);
		}
	}
}