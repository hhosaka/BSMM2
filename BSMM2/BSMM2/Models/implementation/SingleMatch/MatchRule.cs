using BSMM2.Models;
using BSMM2.Modules.Rules.Match;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.RESULT;

namespace BSMM2.Modules.Rules.SingleMatch {

	[JsonObject]
	public class MatchRule : Rule {

		protected override int Compare(IMatchResult x, IMatchResult y, int level) {
			switch (level) {
				case 0:
					return Compare(true, true);

				case 1:
					return Compare(true, false);

				case 2:
					return Compare(false, false);

				default:
					throw new ArgumentException();
			}

			int Compare(bool countLifePoint, bool countWinPoint) {
				int result = 0;
				if (x != y) {
					result = x.MatchPoint - y.MatchPoint;
					if (result == 0 && countLifePoint) {
						result = x.LifePoint - y.LifePoint;
						if (result == 0 && countWinPoint) {
							result = ConvDouble2Int(x.WinPoint - y.WinPoint);
						}
					}
				}
				return result;

				int ConvDouble2Int(double value) {
					if (value == 0.0) {
						return 0;
					} else if (value > 0.0) {
						return 1;
					} else {
						return -1;
					}
				}
			}
		}

		public (IMatchResult, IMatchResult) CreatePoints(RESULT player1Result) {
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