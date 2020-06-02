﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using static BSMM2.Models.RESULT_T;

namespace BSMM2.Models.Matches.SingleMatch {

	[JsonObject]
	public class SingleMatchRule : Rule {
		private const int DEFAULT_LIFE_POINT = 5;

		public override (IResult, IResult) CreatePoints(RESULT_T result)
			=> CreatePoints(result, DEFAULT_LIFE_POINT, DEFAULT_LIFE_POINT);

		public (IResult, IResult) CreatePoints(RESULT_T result1, int lp1, int lp2) {
			if (!EnableLifePoint) {
				lp1 = lp2 = DEFAULT_LIFE_POINT;//  TODO : bad idea
			}
			switch (result1) {
				case Win:
					return (new SingleMatchResult(Win, lp1), new SingleMatchResult(Lose, lp2));

				case Lose:
					return (new SingleMatchResult(Lose, lp1), new SingleMatchResult(Win, lp2));

				case Draw:
					return (new SingleMatchResult(Draw, lp1), new SingleMatchResult(Draw, lp2));

				case Progress:
					return (new SingleMatchResult(Progress, -1), new SingleMatchResult(Draw, -1));

				default:
					throw new ArgumentException();
			}
		}

		public override ContentPage CreateMatchPage(Game game, Match match) {
			Debug.Assert(game.Rule is SingleMatchRule);
			return new SingleMatchPage(game.Rule as SingleMatchRule, match);
		}

		public override Rule Clone()
			=> new SingleMatchRule(this);

		public override Match CreateMatch(IPlayer player1, IPlayer player2)
			=> new Match(this, player1, player2);

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