using Newtonsoft.Json;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.MultiMatch.ThreeGameMatch {

	[JsonObject]
	public class ThreeGameMatchRule : MultiMatchRule {

		[JsonIgnore]
		public override string Name
			=> "Three Game Match Rule";

		[JsonIgnore]
		public override string Description
			=> "二本先取のゲームルールです";

		protected override int MatchCount
			=> 2;

		protected override int MinimumMatchCount
			=> 0;

		public override ContentPage CreateMatchPage(Game game, Match match) {
			throw new System.NotImplementedException();
		}

		public override Rule Clone() {
			throw new System.NotImplementedException();
		}

		public override Match CreateMatch(IPlayer player1, IPlayer player2)
			=> new Match(this, player1, player2);

		public ThreeGameMatchRule() {
		}
	}
}