using BSMM2.Models;
using BSMM2.Modules.Rules.SingleMatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.Rule;

namespace BSMM2.Modules.Rules.Match {

	[JsonObject]
	public abstract class Result : IComparable<Result>, IResult {
		public abstract int MatchPoint { get; }

		public abstract int LifePoint { get; }

		public abstract double WinPoint { get; }

		public static Func<IResult, IResult, int>[] Compareres { get; } = new Func<IResult, IResult, int>[]{
			(p1,p2)=> ((Result)p1).CompareTo((Result)p2)
		};

		public static Result Sum(IEnumerable<Result> points) {
			int matchPoint = 0;
			int lifePoint = 0;

			if (points.Any()) {
				foreach (var point in points) {
					matchPoint += point?.MatchPoint ?? 0;
					lifePoint += point?.LifePoint ?? 0;
				}
				return new SingleMatchResult(matchPoint, (double)matchPoint / points.Count(), lifePoint);
			}
			return new SingleMatchResult(0, 0, 0);
		}

		public int CompareTo(Result other)
			=> CompareTo(other, false, false);

		private int CompareTo(Result other, bool ignoreLifePoint, bool ignoreWinPoint) {
			int result = 0;
			if (this != other) {
				result = MatchPoint - other.MatchPoint;
				if (result == 0) {
					result = ignoreLifePoint ? 0 : LifePoint - other.LifePoint;
					if (result == 0) {
						result = ignoreWinPoint ? 0 : ConvDouble2Int(WinPoint - other.WinPoint);
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
}