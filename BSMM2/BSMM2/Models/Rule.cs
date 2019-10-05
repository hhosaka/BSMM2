using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
		public static Player BYE = new Player("BYE");
		internal abstract IComparer[] Comparers { get; }

		public abstract (IResult, IResult) CreatePoints(RESULT result);

		public abstract IResult Sum(IEnumerable<IResult> results);

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

				int ToComp(RESULT? result)
					=> result == RESULT.Win ? 1 : result == RESULT.Lose ? -1 : 0;

				int CheckDropped() {
					if (x.Dropped)
						return y.Dropped ? 0 : -1;
					else
						return y.Dropped ? 1 : 0;
				}
			}
		}

		public virtual ContentPage ContentPage { get; }

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
	}
}