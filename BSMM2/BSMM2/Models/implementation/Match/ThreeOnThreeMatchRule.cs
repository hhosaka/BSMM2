using Newtonsoft.Json;

namespace BSMM2.Models.Rules.Match {

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
	}
}