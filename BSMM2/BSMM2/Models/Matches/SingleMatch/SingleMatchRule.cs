﻿using Newtonsoft.Json;
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

		public override Rule Clone()
			=> new SingleMatchRule(this);

		public override Match CreateMatch(IPlayer player1, IPlayer player2)
			=> new SingleMatch(this, player1, player2);

		public override IPoint Point(IEnumerable<IPoint> results) {
			return SingleMatchResult.Total(results.Select(result => (IPoint)result));
		}

		public override string Name
			=> "Single Match Rule";

		public override string Description
			=> "一本取りです。";

		public SingleMatchRule() : base() {
			_bye = new Player(this, "BYE");
		}

		public SingleMatchRule(Rule src) : base(src) {
		}
	}
}