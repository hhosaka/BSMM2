using Newtonsoft.Json;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.MultiMatch.ThreeOnThreeMatch {

	[JsonObject]
	public class ThreeOnThreeMatchRule : MultiMatchRule {

		[JsonIgnore]
		public override string Name
			=> "Three on Three Match Rule";

		[JsonIgnore]
		public override string Description
			=> "３on３ゲームのルールです";

		protected override int MatchCount
			=> 3;

		protected override int MinimumMatchCount
			=> 3;

		public override ContentPage CreateMatchPage(Game game, Match match) {
			throw new System.NotImplementedException();
		}

		public override Rule Clone() {
			throw new System.NotImplementedException();
		}
	}
}