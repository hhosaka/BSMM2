using BSMM2.Resource;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	[JsonObject]
	public class SingleMatchRule : Rule {

		[JsonProperty]
		private IPlayer _bye;

		[JsonIgnore]
		public override IPlayer BYE => _bye;

		public override ContentPage CreateMatchPage(Game game, Match match) {
			Debug.Assert(game.Rule is SingleMatchRule);
			return new SingleMatchPage((SingleMatchRule)game.Rule, (SingleMatch)match);
		}

		public override ContentPage CreateRulePage(Game game) {
			Debug.Assert(game.Rule is SingleMatchRule);
			return new SingleMatchRulePage(game);
		}

		public override Rule Clone()
			=> new SingleMatchRule(this);

		public override Match CreateMatch(IPlayer player1, IPlayer player2)
			=> new SingleMatch(this, player1, player2);

		public override IExportablePoint Point(IEnumerable<IPoint> results) {
			return SingleMatchResult.Total(results.Select(result => (IPoint)result));
		}

		public override string Name
			=> AppResources.ItemRuleSingleMatch;

		public override string Description
			=> AppResources.DescriptionSingleMatch;

		public SingleMatchRule() : base() {
			_bye = new Player(this, AppResources.TextBYE);
		}

		public SingleMatchRule(Rule src) : base(src) {
		}
	}
}