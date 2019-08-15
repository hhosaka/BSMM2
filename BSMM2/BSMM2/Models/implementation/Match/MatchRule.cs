using BSMM2.Models;
using BSMM2.Models.Rules.Match;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.RESULT;

namespace BSMM2.Models.Rules.Match {

	internal class MatchResultTotal : IMatchResult {

		[JsonProperty]
		public int MatchPoint { get; }

		[JsonProperty]
		public int LifePoint { get; }

		[JsonProperty]
		public double WinPoint { get; }

		[JsonIgnore]
		public int Point => MatchPoint;

		public MatchResultTotal(IEnumerable<IResult> source) {
			if (source.Any()) {
				foreach (var data in source) {
					var point = data as IMatchResult;
					if (point != null) {
						MatchPoint += point?.MatchPoint ?? 0;
						LifePoint += point?.LifePoint ?? 0;
					}
				}
				WinPoint = (double)MatchPoint / source.Count();
			}
		}
	}

	internal class MatchResult : IMatchResult {

		[JsonIgnore]
		public int MatchPoint
			=> ResultValue == RESULT.Win ? 3 : ResultValue == RESULT.Lose ? 0 : 1;

		[JsonIgnore]
		public double WinPoint
			=> ResultValue == RESULT.Win ? 1.0 : ResultValue == RESULT.Lose ? 0.0 : 0.5;

		[JsonProperty]
		public int LifePoint { get; }

		[JsonProperty]
		private RESULT ResultValue { get; }

		[JsonIgnore]
		public int Point => MatchPoint;

		public MatchResult(RESULT result, int lifePoint = 0) {
			ResultValue = result;
			LifePoint = lifePoint;
		}
	}

	[JsonObject]
	public interface IMatchResult : IResult {
		int MatchPoint { get; }

		int LifePoint { get; }

		double WinPoint { get; }
	}

	[JsonObject]
	public class MatchRule : Rule {

		protected override int Compare(IMatchResult x, IMatchResult y, int level) {
			switch (level) {
				case 0:
					return Compare(true, true);

				case 1:
					return Compare(true, false);

				case 2:
					return Compare(false, false);

				default:
					throw new ArgumentException();
			}

			int Compare(bool countLifePoint, bool countWinPoint) {
				int result = 0;
				if (x != y) {
					result = x.MatchPoint - y.MatchPoint;
					if (result == 0 && countLifePoint) {
						result = x.LifePoint - y.LifePoint;
						if (result == 0 && countWinPoint) {
							result = ConvDouble2Int(x.WinPoint - y.WinPoint);
						}
					}
				}
				return result;

				int ConvDouble2Int(double value) {
					if (value == 0.0) {
						return 0;
					} else if (value > 0.0) {
						return 1;
					} else {
						return -1;
					}
				}
			}
		}

		public (IMatchResult, IMatchResult) CreatePoints(RESULT player1Result) {
			switch (player1Result) {
				case Win:
					return (new MatchResult(Win), new MatchResult(Lose));

				case Lose:
					return (new MatchResult(Lose), new MatchResult(Win));

				case Draw:
					return (new MatchResult(Draw), new MatchResult(Draw));

				default:
					throw new ArgumentException();
			}
		}

		public override ContentPage ContentPage
			=> null;
	}
}