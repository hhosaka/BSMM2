using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public abstract class Rule {

		[JsonProperty]
		public bool EnableLifePoint { get; set; }

		[JsonProperty]
		private IComparer[] _comparers;

		[JsonIgnore]
		public IEnumerable<IComparer> Comparers => _comparers;

		[JsonIgnore]
		public int CompareDepth => _comparers.Count();

		[JsonIgnore]
		public abstract string Name { get; }

		[JsonIgnore]
		public abstract string Description { get; }

		private class TheComparer : Comparer<Player> {
			private IEnumerable<IComparer> _compareres;
			private bool _force;

			public TheComparer(IEnumerable<IComparer> compareres, bool force) {
				_compareres = compareres;
				_force = force;
			}

			public override int Compare(Player p1, Player p2) {
				foreach (var c in _compareres.Where(c => _force || c.Active)) {
					var ret = c.Compare(p1, p2);
					if (ret != 0) return ret;
				}
				return 0;
			}
		}

		public abstract Match CreateMatch(IPlayer player1, IPlayer player2 = null);

		public abstract ContentPage CreateMatchPage(Match match);

		public abstract ContentPage CreateRulePage(Game game);

		public abstract Rule Clone();

		public abstract IExportablePoint Point(IEnumerable<IPoint> results);

		public abstract IPlayer BYE { get; }

		public Comparer<Player> GetComparer(bool force)
			=> new TheComparer(_comparers, force);

		public Rule() {
			_comparers = new IComparer[] {
				new PreComparer(),
				new PointComparer(),
				new LifePointComparer(),
				new OpponentMatchPointComparer(),
				new OpponentLifePointComparer(),
				new WinPointComparer(),
				new OpponentWinPointComparer(),
				new PostComparer(),
			};
		}

		public Rule(Rule src) : this() {
			EnableLifePoint = src.EnableLifePoint;
		}
	}
}