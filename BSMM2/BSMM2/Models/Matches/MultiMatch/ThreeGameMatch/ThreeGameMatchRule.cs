using BSMM2.Resource;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.MultiMatch.ThreeGameMatch {

	[JsonObject]
	public class ThreeGameMatchRule : MultiMatchRule {

		[JsonIgnore]
		public override string Name
			=> AppResources.ItemRuleThreeGameMatch;

		[JsonIgnore]
		public override string Description
			=> AppResources.DescriptionTreeGameMatch;

		public override ContentPage CreateMatchPage(Game game, Match match) {
			throw new System.NotImplementedException();
		}

		public override Rule Clone() {
			throw new System.NotImplementedException();
		}

		public override Match CreateMatch(IPlayer player1, IPlayer player2)
			=> new MultiMatch(this, player1, player2);

		public ThreeGameMatchRule() : base(2, 0) {
		}
	}
}