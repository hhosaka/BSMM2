using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	internal interface IComparer {
		string Name { get; }
		string Description { get; }
		bool Active { get; set; }

		int Compare(Player p1, Player p2);
	}

	[JsonObject]
	public abstract class Rule {

		[JsonProperty]
		public bool EnableLifePoint { get; set; }

		[JsonProperty]
		public bool AcceptByeMatchDuplication { get; set; }

		[JsonProperty]
		public bool AcceptGapMatchDuplication { get; set; }

		private static int ConvDouble2Int(double value) {
			if (value == 0.0) {
				return 0;
			} else if (value > 0.0) {
				return 1;
			} else {
				return -1;
			}
		}

		private class PointComparer : IComparer {

			public string Name
				=> "Match Point";

			public string Description
				=> "合計得点";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> p1.Result.Point - p2.Result.Point;
		}

		private class LifePointComparer : IComparer {

			public string Name
				=> "Life Point";

			public string Description
				=> "合計ライフポイント";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> p1.Result.LifePoint - p2.Result.LifePoint;
		}

		private class OpponentMatchPointComparer : IComparer {

			public string Name
				=> "Opponent Match Point";

			public string Description
				=> "対戦相手の合計得点";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> p1.OpponentResult.Point - p2.OpponentResult.Point;
		}

		private class OpponentLifePointComparer : IComparer {

			public string Name
				=> "Opponent Life Point";

			public string Description
				=> "対戦相手の合計ライフポイント";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> p1.OpponentResult.LifePoint - p2.OpponentResult.LifePoint;
		}

		private class WinPointComparer : IComparer {

			public string Name
				=> "Win Point";

			public string Description
				=> "合計勝利ポイント";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> ConvDouble2Int(p1.Result.WinPoint - p2.Result.WinPoint);
		}

		private class OpponentWinPointComparer : IComparer {

			public string Name
				=> "Opponent Win Point";

			public string Description
				=> "対戦相手の合計勝利ポイント";

			public bool Active { get; set; } = true;

			public int Compare(Player p1, Player p2)
				=> ConvDouble2Int(p1.OpponentResult.WinPoint - p2.OpponentResult.WinPoint);
		}

		private class ByePlayer : IPlayer {
			public string Name => "BYE";

			public bool Dropped => true;

			public IResult Result { get; }

			public IResult OpponentResult => Result;

			public bool HasByeMatch => false;

			public bool HasGapMatch => false;

			public void Commit(Match match) {
			}

			public void ExportTitle(TextWriter writer) {
				throw new NotImplementedException();
			}

			public void ExportData(TextWriter writer) {
				throw new NotImplementedException();
			}

			public ByePlayer() {
				Result = new SingleMatchResult(RESULT_T.Lose, 0);
			}
		}

		public static IPlayer BYE = new ByePlayer();
		internal IComparer[] Comparers { get; }

		public abstract (IResult, IResult) CreatePoints(RESULT_T result);

		public int CompareDepth => Comparers.Count();

		[JsonIgnore]
		public abstract string Name { get; }

		[JsonIgnore]
		public abstract string Description { get; }

		private class TheComparer : Comparer<Player> {
			private Func<Player, Player, int> _comparer;

			public TheComparer(Func<Player, Player, int> comparer) {
				_comparer = comparer;
			}

			public override int Compare(Player x, Player y) {
				int ret = 0;
				if (x == y) {
					return 0;
				} else {
					ret = CheckDropped();
					if (ret == 0) {
						if (x.Result != null && y.Result != null) {
							ret = _comparer(x, y);
							if (ret == 0) {
								return ToComp(x.GetResult(y));
							}
						}
					}
				}
				return ret;

				int ToComp(RESULT_T? result)
					=> result == RESULT_T.Win ? 1 : result == RESULT_T.Lose ? -1 : 0;

				int CheckDropped() {
					if (x.Dropped)
						return y.Dropped ? 0 : -1;
					else
						return y.Dropped ? 1 : 0;
				}
			}
		}

		public abstract Match CreateMatch(IPlayer player1, IPlayer player2 = null);

		public abstract ContentPage CreateMatchPage(Game game, Match match);

		public abstract Rule Clone();

		public Comparer<Player> CreateOrderComparer()
			=> new TheComparer((p1, p2) => Compare(p1, p2, true));

		public Comparer<Player> CreateSourceComparer()
			=> new TheComparer((p1, p2) => Compare(p1, p2, false));

		private int Compare(Player sx, Player sy, bool fully) {
			foreach (var comparer in Comparers) {
				if (fully || comparer.Active) {
					var ret = comparer.Compare(sx, sy);
					if (ret != 0) return ret;
				}
			}
			return 0;
		}

		public Rule() {
			Comparers = new IComparer[] {
				new PointComparer(),
				new LifePointComparer(),
				new OpponentMatchPointComparer(),
				new OpponentLifePointComparer(),
				new WinPointComparer(),
				new OpponentWinPointComparer(),
			};
		}

		public Rule(Rule src) : this() {
			EnableLifePoint = src.EnableLifePoint;
			AcceptByeMatchDuplication = src.AcceptByeMatchDuplication;
			AcceptGapMatchDuplication = src.AcceptGapMatchDuplication;
		}
	}
}