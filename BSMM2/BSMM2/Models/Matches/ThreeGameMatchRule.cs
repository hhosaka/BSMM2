using Newtonsoft.Json;

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
	}
}