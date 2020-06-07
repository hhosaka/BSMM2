using Newtonsoft.Json;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	[JsonObject]
	public class SingleMatchRule : Rule {

		public override ContentPage CreateMatchPage(Game game, Match match) {
			Debug.Assert(game.Rule is SingleMatchRule);
			return new SingleMatchPage((SingleMatchRule)game.Rule, (SingleMatch)match);
		}

		public override Rule Clone()
			=> new SingleMatchRule(this);

		public override Match CreateMatch(IPlayer player1, IPlayer player2)
			=> new SingleMatch(this, player1, player2);

		public override string Name
			=> "Single Match Rule";

		public override string Description
			=> "一本取りです。";

		public SingleMatchRule() : base() {
		}

		public SingleMatchRule(Rule src) : base(src) {
		}
	}
}