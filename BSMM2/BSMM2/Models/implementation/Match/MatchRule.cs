using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using static BSMM2.Models.RESULT;

namespace BSMM2.Models.Rules.Match {

	[JsonObject]
	public class MatchRule : Rule {

		public class MatchPoint : IComparer {

			public string Name
				=> "Match Point";

			public string Description
				=> "合計得点";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> Result(p1).MatchPoint - Result(p2).MatchPoint;
		}

		public class LifePoint : IComparer {

			public string Name
				=> "Life Point";

			public string Description
				=> "合計ライフポイント";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> Result(p1).LifePoint - Result(p2).LifePoint;
		}

		public class OpponentMatchPoint : IComparer {

			public string Name
				=> "Opponent Match Point";

			public string Description
				=> "対戦相手の合計得点";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> OpponentResult(p1).MatchPoint - OpponentResult(p2).MatchPoint;
		}

		public class OpponentLifePoint : IComparer {

			public string Name
				=> "Opponent Life Point";

			public string Description
				=> "対戦相手の合計ライフポイント";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> OpponentResult(p1).LifePoint - OpponentResult(p2).LifePoint;
		}

		public class WinPoint : IComparer {

			public string Name
				=> "Win Point";

			public string Description
				=> "合計勝利ポイント";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> ConvDouble2Int(Result(p1).WinPoint - Result(p2).WinPoint);
		}

		public class OpponentWinPoint : IComparer {

			public string Name
				=> "Opponent Win Point";

			public string Description
				=> "対戦相手の合計勝利ポイント";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> ConvDouble2Int(OpponentResult(p1).WinPoint - OpponentResult(p2).WinPoint);
		}

		private class Total : IMatchResult {

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

			public Total(IEnumerable<IMatchResult> source) {
				foreach (var data in source) {
					var point = data as IMatchResult;
					if (point != null) {
						MatchPoint += point.MatchPoint;
						LifePoint += point.LifePoint;
						WinPoint += point.WinPoint;
					}
				}
				WinPoint = source.Any() ? WinPoint / source.Count() : 0.0;
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

		private static IMatchResult Result(Player player)
			=> (IMatchResult)player.Result;

		private static IMatchResult OpponentResult(Player player)
			=> (IMatchResult)player.OpponentResult;

		internal override IComparer[] Comparers { get; }

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
			return new Total(results.Cast<IMatchResult>());
		}

		public override ContentPage ContentPage
			=> null;

		public override string Name
			=> "Single Match Rule";

		public override string Description
			=> "一本取りです。";

		public MatchRule() {
			Comparers = new IComparer[] {
				new MatchPoint(),
				new LifePoint(),
				new OpponentMatchPoint(),
				new OpponentLifePoint(),
				new WinPoint(),
				new OpponentWinPoint(),
			};
		}
	}
}