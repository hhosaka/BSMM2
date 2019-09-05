using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static BSMM2.Models.RESULT;

namespace BSMM2.Models.Rules.Match {

	[JsonObject]
	public class MultiMatchRule : MatchRule {

		[JsonIgnore]
		public override string Name
			=> "Multi Match Point";

		[JsonIgnore]
		public override string Description
			=> "二本取り以上のゲームルールです";

		[JsonProperty]
		private int _count;

		[JsonProperty]
		private int _minCount;

		public override (IResult, IResult) CreatePoints(RESULT result) {
			var results = new List<RESULT>();
			for (int i = 0; i < _count; ++i) {
				results.Add(result);
			}
			return CreatePoints(results.ToArray());
		}

		public (IResult, IResult) CreatePoints(IEnumerable<RESULT> player1Results) {
			return CreatePoints(player1Results.Select(r => (r, 0, 0)));
		}

		public (IResult, IResult) CreatePoints(IEnumerable<(RESULT, int lp1, int lp2)> player1Results) {
			var p1result = new MultiMatchResult(_minCount);
			var p2result = new MultiMatchResult(_minCount);
			foreach (var result in player1Results) {
				switch (result.Item1) {
					case Win:
						p1result.Add(new MatchResult(Win, result.lp1));
						p2result.Add(new MatchResult(Lose, result.lp2));
						break;

					case Lose:
						p1result.Add(new MatchResult(Lose, result.lp1));
						p2result.Add(new MatchResult(Win, result.lp2));
						break;

					case Draw:
						p1result.Add(new MatchResult(Draw, result.lp1));
						p2result.Add(new MatchResult(Draw, result.lp2));
						break;

					default:
						throw new ArgumentException();
				}
			}
			return (p1result, p2result);
		}

		public MultiMatchRule(int count, int minCount) {
			_count = count;
			_minCount = minCount;
		}
	}
}