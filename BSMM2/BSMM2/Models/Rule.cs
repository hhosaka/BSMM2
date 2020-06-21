using Newtonsoft.Json;
using System.Collections.Generic;
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
		public abstract string Name { get; }

		[JsonIgnore]
		public abstract string Description { get; }

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