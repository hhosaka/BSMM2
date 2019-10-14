using Newtonsoft.Json;
using System;
using Xamarin.Forms;
using static BSMM2.Models.RESULT_T;

namespace BSMM2.Models.Matches {

	[JsonObject]
	public class SingleMatchRule : Rule {
		private const int DEFAULT_LIFE_POINT = 5;

		public override (IResult, IResult) CreatePoints(RESULT_T result)
			=> CreatePoints(result, DEFAULT_LIFE_POINT, DEFAULT_LIFE_POINT);

		public (IResult, IResult) CreatePoints(RESULT_T result1, int lp1, int lp2) {
			switch (result1) {
				case Win:
					return (new SingleMatchResult(Win, lp1), new SingleMatchResult(Lose, lp2));

				case Lose:
					return (new SingleMatchResult(Lose, lp1), new SingleMatchResult(Win, lp2));

				case Draw:
					return (new SingleMatchResult(Draw, lp1), new SingleMatchResult(Draw, lp2));

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