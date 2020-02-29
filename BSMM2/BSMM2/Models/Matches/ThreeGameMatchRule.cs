using Newtonsoft.Json;
using Xamarin.Forms;

namespace BSMM2.Models.Matches {

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

		public override ContentPage CreateMatchPage(Game game, IMatch match) {
			throw new System.NotImplementedException();
		}

		public override Rule Clone() {
			throw new System.NotImplementedException();
		}

		public ThreeGameMatchRule() {
		}
	}
}