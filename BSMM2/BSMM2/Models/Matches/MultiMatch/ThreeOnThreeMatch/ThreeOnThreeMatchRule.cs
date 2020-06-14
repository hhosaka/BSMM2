using BSMM2.Resource;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.MultiMatch.ThreeOnThreeMatch {

	[JsonObject]
	public class ThreeOnThreeMatchRule : MultiMatchRule {

		[JsonIgnore]
		public override int MatchCount => 3;

		[JsonIgnore]
		public override int MinimumMatchCount => 3;

		[JsonIgnore]
		public override string Name
			=> AppResources.ItemRuleThreeOnThreeMatch;

		[JsonIgnore]
		public override string Description
			=> AppResources.DescriptionThreeOnThreeMatch;

		public override ContentPage CreateMatchPage(Game game, Match match) {
			throw new System.NotImplementedException();
		}

		public override Rule Clone() {
			return new ThreeOnThreeMatchRule(this);
		}

		public override Match CreateMatch(IPlayer player1, IPlayer player2)
			=> new MultiMatch(this, player1, player2);

		public ThreeOnThreeMatchRule() {
		}

		private ThreeOnThreeMatchRule(MultiMatchRule rule) : base(rule) {
		}
	}
}