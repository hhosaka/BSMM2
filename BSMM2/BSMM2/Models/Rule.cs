using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public abstract class Rule {

		[JsonProperty]
		public bool EnableLifePoint { get; set; }

		internal IComparer[] Comparers { get; }

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
						ret = _comparer(x, y);
						if (ret == 0) {
							return ToComp(x.GetResult(y));
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

		public abstract ContentPage CreateMatchPage(Match match);

		public abstract ContentPage CreateRulePage(Game game);

		public abstract Rule Clone();

		public abstract IExportablePoint Point(IEnumerable<IPoint> results);

		public abstract IPlayer BYE { get; }

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
		}
	}
}