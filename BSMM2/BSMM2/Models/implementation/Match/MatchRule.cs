using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using static BSMM2.Models.RESULT;

namespace BSMM2.Models.Rules.Match {

	[JsonObject]
	public class MatchRule : Rule {

		private class MatchResultTotal : IMatchResult {

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

			public bool IsFinished => true;

			public MatchResultTotal(IEnumerable<IResult> source) {
				if (source.Any()) {
					foreach (var data in source) {
						var point = data as IMatchResult;
						if (point != null) {
							MatchPoint += point.MatchPoint;
							LifePoint += point.LifePoint;
							WinPoint += point.WinPoint;
						}
					}
					WinPoint = WinPoint / source.Count();
				}
			}
		}

		private const int DEFAULT_LIFE_POINT = 5;

		public static int ConvDouble2Int(double value) {
			if (value == 0.0) {
				return 0;
			} else if (value > 0.0) {
				return 1;
			} else {
				return -1;
			}
		}

		protected static IMatchResult Result(Player player)
			=> (IMatchResult)player.Result;

		protected static IMatchResult OpponentResult(Player player)
			=> (IMatchResult)player.OpponentResult;

		protected override Func<Player, Player, int>[] Comparers
			=> new Func<Player, Player, int>[] {
						(x, y) => Result(x).MatchPoint - Result(y).MatchPoint,
						(x, y) => Result(x).LifePoint - Result(y).LifePoint,
						(x, y) => OpponentResult(x).MatchPoint - OpponentResult(y).MatchPoint,
						(x, y) => OpponentResult(x).LifePoint - OpponentResult(y).LifePoint,
						(x, y) => ConvDouble2Int(Result(x).WinPoint - Result(y).WinPoint),
						(x, y) => ConvDouble2Int(OpponentResult(x).WinPoint - OpponentResult(y).WinPoint),
			};

		public override (IResult, IResult) CreatePoints(RESULT result)
			=> CreatePoints((result, DEFAULT_LIFE_POINT, DEFAULT_LIFE_POINT));

		public (IResult, IResult) CreatePoints((RESULT result1, int lp1, int lp2) result) {
			switch (result.result1) {
				case Win:
					return (new MatchResult(Win, result.lp1), new MatchResult(Lose, result.lp2));

				case Lose:
					return (new MatchResult(Lose, result.lp1), new MatchResult(Win, result.lp2));

				case Draw:
					return (new MatchResult(Draw, result.lp1), new MatchResult(Draw, result.lp2));

				default:
					throw new ArgumentException();
			}
		}

		public override IResult Sum(IEnumerable<IResult> results) {
			return new MatchResultTotal(results);
		}

		public override ContentPage ContentPage
			=> null;

		public override string Name
			=> "Single Match Rule";

		public override string Description
			=> "一本取りです。";
	}
}