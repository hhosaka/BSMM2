using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public abstract class Rule {
		protected abstract Func<Player, Player, int>[] Comparers { get; }

		public abstract (IResult, IResult) CreatePoints(RESULT result);

		public abstract IResult Sum(IEnumerable<IResult> results);

		public int CompareDepth => Comparers.Count();

		[JsonIgnore]
		public abstract string Name { get; }

		[JsonIgnore]
		public abstract string Description { get; }

		private class TheComparer : Comparer<Player> {
			private Rule _rule;
			private int _level;

			public TheComparer(Rule rule, int level) {
				_rule = rule;
				_level = level;
			}

			public override int Compare(Player x, Player y) {
				int ret = 0;
				if (x == y) {
					return 0;
				} else {
					ret = IsDropped();
					if (ret == 0) {
						if (x.Result != null && y.Result != null) {
							ret = _rule.Compare(x, y, _level);
							if (ret == 0) {
								return ToComp(x.GetResult(y));
							}
						}
					}
				}
				return ret;

				int ToComp(RESULT? result)
					=> result == RESULT.Win ? 1 : result == RESULT.Lose ? -1 : 0;

				int IsDropped() {
					if (x.Dropped)
						return y.Dropped ? 0 : -1;
					else
						return y.Dropped ? 1 : 0;
				}
			}
		}

		public virtual ContentPage ContentPage { get; }

		public Comparer<Player> CreateComparer(int level = 0)
			=> new TheComparer(this, level);

		private int Compare(Player sx, Player sy, int level) {
			foreach (var comparer in Comparers.Take(Comparers.Count() - level)) {
				var ret = comparer(sx, sy);
				if (ret != 0) return ret;
			}
			return 0;
		}
	}
}