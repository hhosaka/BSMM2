using BSMM2.Models;
using BSMM2.Modules.Rules.Match;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.Rule.RESULT;

namespace BSMM2.Modules.Rules.SingleMatch {

	[JsonObject]
	public class MatchRule : Rule {

		public override IEnumerable<Func<IResult, IResult, int>> Compareres
			=> Result.Compareres;

		public (Result, Result) CreatePoints(RESULT player1Result) {
			switch (player1Result) {
				case Win:
					return (new SingleMatchResult(Win), new SingleMatchResult(Lose));

				case Lose:
					return (new SingleMatchResult(Lose), new SingleMatchResult(Win));

				case Draw:
					return (new SingleMatchResult(Draw), new SingleMatchResult(Draw));

				default:
					throw new ArgumentException();
			}
		}

		public override ContentPage ContentPage
			=> null;
	}
}