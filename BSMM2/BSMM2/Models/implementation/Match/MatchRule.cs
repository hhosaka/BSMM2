using Newtonsoft.Json;
using System;
using Xamarin.Forms;
using static BSMM2.Models.RESULT;

namespace BSMM2.Models.Rules.Match {

	[JsonObject]
	public class MatchRule : Rule {
		private const int DEFAULT_LIFE_POINT = 5;

		public override (IResult, IResult) CreatePoints(RESULT result)
			=> CreatePoints((result, DEFAULT_LIFE_POINT, DEFAULT_LIFE_POINT));

		public (IResult, IResult) CreatePoints((RESULT result1, int lp1, int lp2) result) {
			switch (result.result1) {
				case Win:
					return (new MatchResult(Win, result.lp1), new MatchResult(Lose, result.lp2));

				case Lose:
					return (new MatchResult(Lose, result.lp1), new MatchResult(Win, result.lp2));

				case Draw:
					return (new MatchResult(Draw, result.lp1), new MatchResult(Draw, result.lp2));

				default:
					throw new ArgumentException();
			}
		}

		public override ContentPage ContentPage
			=> null;

		public override string Name
			=> "Single Match Rule";

		public override string Description
			=> "一本取りです。";
	}
}