using BSMM2.Models;
using BSMM2.Modules.Rules.SingleMatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.Rule;

namespace BSMM2.Modules.Rules {

	[JsonObject]
	public abstract class Result : IComparable<Result> {
		public abstract int MatchPoint { get; }

		public abstract int LifePoint { get; }

		public abstract double WinPoint { get; }

		public static Func<object, object, int>[] Compareres { get; } = new Func<object, object, int>[]{
			(p1,p2)=> ((Result)p1).MatchPoint - ((Result)p2).MatchPoint,
			(p1,p2)=> ((Result)p1).LifePoint - ((Result)p2).LifePoint,
			(p1,p2)=> {var result=((Result)p1).WinPoint - ((Result)p2).WinPoint; return result>0?1:result<0?-1:0; },
		};

		public static Result Sum(IEnumerable<Result> points) {
			int matchPoint = 0;
			int lifePoint = 0;

			foreach (var point in points) {
				matchPoint += point?.MatchPoint ?? 0;
				lifePoint += point?.LifePoint ?? 0;
			}
			return new SingleMatchResult(matchPoint, (double)matchPoint / points.Count(), lifePoint);
		}

		public int CompareTo(Result other) {
			if (this != other) {
				foreach (var comparer in Compareres) {
					var result = comparer(this, other);
					if (result != 0) {
						return result;
					}
				}
			}
			return 0;
		}
	}
}