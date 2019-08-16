using BSMM2.Models.Rules;
using BSMM2.Models.Rules.Match;
using BSMM2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public abstract class Rule {

		protected abstract int Compare(IMatchResult x, IMatchResult y, int level);

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
					ret = Dropped();
					if (ret == 0) {
						ret = _rule.Compare(x.Result, y.Result, _level);
						if (ret == 0) {
							return ToComp(x.GetResult(y));
						}
					}
				}
				return ret;

				int ToComp(RESULT? result)
					=> result == RESULT.Win ? 1 : result == RESULT.Lose ? -1 : 0;

				int Dropped() {
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
	}
}