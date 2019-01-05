using BSMM2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.Rule;

namespace BSMM2.Modules.Rules.SingleMatch {

	[JsonObject]
	public class SingleMatchResult : Result {

		[JsonProperty]
		public override int MatchPoint { get; }

		[JsonProperty]
		public override int LifePoint { get; }

		[JsonIgnore]
		public override double WinPoint { get; }

		private static double Result2WinPoint(RESULT result) {
			switch (result) {
				case RESULT.Win:
					return 1.0;

				case RESULT.Lose:
					return 0;

				case RESULT.Draw:
					return 0.5;
			}
			throw new ArgumentException();
		}

		private static int Result2MatchPoint(RESULT result) {
			switch (result) {
				case RESULT.Win:
					return 3;

				case RESULT.Lose:
					return 0;

				case RESULT.Draw:
					return 1;
			}
			throw new ArgumentException();
		}

		private SingleMatchResult() {
		}

		public SingleMatchResult(int matchPoint, double winPoint, int lifePoint = 0) {
			MatchPoint = matchPoint;
			WinPoint = winPoint;
			LifePoint = lifePoint;
		}

		public SingleMatchResult(RESULT result, int lifePoint = 0)
			: this(Result2MatchPoint(result), Result2WinPoint(result), lifePoint) {
		}
	}
}