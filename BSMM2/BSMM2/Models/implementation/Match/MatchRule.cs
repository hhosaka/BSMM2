using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
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

		[JsonIgnore]
		public RESULT? RESULT => null;

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
			=> RESULT == Models.RESULT.Win ? 3 : RESULT == Models.RESULT.Lose ? 0 : 1;

		[JsonIgnore]
		public double WinPoint
			=> RESULT == Models.RESULT.Win ? 1.0 : RESULT == Models.RESULT.Lose ? 0.0 : 0.5;

		[JsonProperty]
		public int LifePoint { get; }

		[JsonProperty]
		public RESULT? RESULT { get; }

		[JsonIgnore]
		public int Point => MatchPoint;

		public MatchResult(RESULT result, int lifePoint = 0) {
			RESULT = result;
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
		public override int CompareDepth => 3;

		private static int ConvDouble2Int(double value) {
			if (value == 0.0) {
				return 0;
			} else if (value > 0.0) {
				return 1;
			} else {
				return -1;
			}
		}

		private static IMatchResult Result(Player player)
			=> (IMatchResult)player.Result;

		private static IMatchResult OpponentResult(Player player)
			=> (IMatchResult)player.OpponentResult;

		protected override Func<Player, Player, int>[] Comparers
			=> new Func<Player, Player, int>[] {
						(x, y) => Result(x).MatchPoint - Result(y).MatchPoint,
						(x, y) => Result(x).LifePoint - Result(y).LifePoint,
						(x, y) => OpponentResult(x).MatchPoint - OpponentResult(y).MatchPoint,
						(x, y) => OpponentResult(x).LifePoint - OpponentResult(y).LifePoint,
						(x, y) => ConvDouble2Int(Result(x).WinPoint - Result(y).WinPoint),
			};

		public (IResult, IResult) CreatePoints(RESULT player1Result) {
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

		public override IResult Sum(IEnumerable<IResult> results) {
			return new MatchResultTotal(results);
		}

		public override ContentPage ContentPage
			=> null;
	}
}