using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static BSMM2.Models.RESULT_T;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	public class MultiMatchResult : IResult, IBSPoint {

		[JsonProperty]
		private List<IResult> _results;

		[JsonProperty]
		private int _minCount;

		[JsonIgnore]
		private RESULT_T _RESULT;

		[JsonIgnore]
		public int LifePoint
			=> _results.Sum(p => ((IBSPoint)p).LifePoint);

		[JsonIgnore]
		public double WinPoint
			=> _results.Sum(p => p.WinPoint) / _results.Count();

		[JsonIgnore]
		public RESULT_T RESULT
			=> _RESULT = GetResult();

		public int MatchPoint
			=> RESULT == Win ? 3 : RESULT == Lose ? 0 : 1;

		[JsonIgnore]
		public bool IsFinished
			=> _results.Count(result => result.IsFinished) >= _minCount;

		[JsonIgnore]
		public IEnumerable<IBSPoint> Results => _results.Cast<IBSPoint>();

		public void Clear()
			=> _results.Clear();

		public void Add(IResult result)
			=> _results.Add(result);

		private RESULT_T GetResult() {
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
			return RESULT_T.Progress;
		}

		public void ExportTitle(TextWriter writer) {
			throw new System.NotImplementedException();
		}

		public void ExportData(TextWriter writer) {
			throw new System.NotImplementedException();
		}

		public IPoint GetPoint() => this;

		public int? CompareTo(IPoint point, int strictness = 0) {
			//if (strictness == 0) {// TODO
			return MatchPoint - point.MatchPoint;
			//}
		}

		public MultiMatchResult(int minCount) {
			_minCount = minCount;
			_results = new List<IResult>();
		}
	}
}