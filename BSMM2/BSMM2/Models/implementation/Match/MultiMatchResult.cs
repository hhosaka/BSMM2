using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static BSMM2.Models.RESULT;

namespace BSMM2.Models.Rules.Match {

	[JsonObject]
	public class MultiMatchResult : IMatchResult {

		[JsonProperty]
		private List<IMatchResult> _results;

		[JsonProperty]
		private int _minCount;

		[JsonIgnore]
		private RESULT? _RESULT;

		[JsonIgnore]
		public int MatchPoint
			=> RESULT == Win ? 3 : RESULT == Lose ? 0 : 1;

		[JsonIgnore]
		public int LifePoint
			=> _results.Sum(p => p.LifePoint);

		[JsonIgnore]
		public double WinPoint
			=> _results.Sum(p => p.WinPoint) / _results.Count();

		[JsonIgnore]
		public RESULT? RESULT
			=> _RESULT ?? (_RESULT = GetResult());

		public int Point
			=> MatchPoint;

		public bool IsFinished => _results.Count() >= _minCount;

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

		public MultiMatchResult(int minCount) {
			_minCount = minCount;
			_results = new List<IMatchResult>();
		}
	}
}