using BSMM2.Resource;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	[JsonObject]
	public class SingleMatchRule : IRule {

		[JsonProperty]
		public bool EnableLifePoint { get; set; }

		[JsonProperty]
		private IEnumerable<IComparer> _comparers;

		[JsonIgnore]
		public IEnumerable<IComparer> Comparers => _comparers;

		[JsonIgnore]
		public IPlayer BYE { get; }

		public virtual ContentPage CreateMatchPage(Match match)
			=> new SingleMatchPage(this, (SingleMatch)match);

		public ContentPage CreateRulePage(Game game)
			=> new SingleMatchRulePage(game);

		public virtual IRule Clone()
			=> new SingleMatchRule(this);

		public virtual Match CreateMatch(IPlayer player1, IPlayer player2)
			=> new SingleMatch(this, player1, player2);

		public IExportablePoint Point(IEnumerable<IPoint> results) {
			return SingleMatchResult.Total(results.Select(result => (IPoint)result));
		}

		public virtual Comparer<Player> GetComparer(bool force)
			=> new TheComparer(Comparers, force);

		public virtual string Name
			=> AppResources.ItemRuleSingleMatch;

		public virtual string Description
			=> AppResources.DescriptionSingleMatch;

		public SingleMatchRule() : base() {
			BYE = new Player(this, AppResources.TextBYE);
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

		protected SingleMatchRule(SingleMatchRule src) : this() {
			EnableLifePoint = src.EnableLifePoint;
		}
	}
}