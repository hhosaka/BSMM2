using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static BSMM2.Models.RESULT;

namespace BSMM2.Models.Rules.Match {

	[JsonObject]
	public class MultiMatchRule : MatchRule {

		public override (IResult, IResult) ByePoints
			=> CreatePoints(Win);

		public override string Name
			=> "Multi Match Point";

		public override string Description
			=> "二本取り以上のゲームルールです";

		public (IResult, IResult) CreatePoints(IEnumerable<RESULT> player1Results) {
			return CreatePoints(player1Results.Select(r => (r, 0, 0)));
		}

		public (IResult, IResult) CreatePoints(IEnumerable<(RESULT, int, int)> player1Results) {
			var p1result = new MultiMatchResult();
			var p2result = new MultiMatchResult();
			foreach (var result in player1Results) {
				switch (result.Item1) {
					case Win:
						p1result.Add(new MatchResult(Win, result.Item2));
						p2result.Add(new MatchResult(Lose, result.Item3));
						break;

					case Lose:
						p1result.Add(new MatchResult(Lose, result.Item2));
						p2result.Add(new MatchResult(Win, result.Item3));
						break;

					case Draw:
						p1result.Add(new MatchResult(Draw, result.Item2));
						p2result.Add(new MatchResult(Draw, result.Item3));
						break;

					default:
						throw new ArgumentException();
				}
			}
			return (p1result, p2result);
		}
	}

	[JsonObject]
	public class MultiMatchResult : IMatchResult {

		[JsonProperty]
		private List<IMatchResult> _results;

		[JsonIgnore]
		private RESULT? _RESULT;

		[JsonIgnore]
		public int MatchPoint
			=> _results.Sum(p => p.MatchPoint);

		[JsonIgnore]
		public int LifePoint
			=> _results.Sum(p => p.LifePoint);

		[JsonIgnore]
		public double WinPoint
			=> _results.Sum(p => p.WinPoint);

		[JsonIgnore]
		public RESULT? RESULT
			=> _RESULT ?? (_RESULT = GetResult());

		public int Point
			=> MatchPoint;

		public void Add(IMatchResult result) {
			_RESULT = null;
			_results.Add(result);
		}

		private RESULT? GetResult() {
			if (_results.Any()) {
				int result = 0;
				foreach (var r in _results) {
					switch (r.RESULT) {
						case Win:
							++result;
							break;

						case Lose:
							--result;
							break;
					}
				}
				return result == 0 ? Draw : result > 0 ? Win : Lose;
			}
			return null;
		}

		public MultiMatchResult() {
			_results = new List<IMatchResult>();
		}
	}
}