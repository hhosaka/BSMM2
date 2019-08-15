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

	internal class MatchResultTotal : IMatchResult {

		[JsonProperty]
		public int MatchPoint { get; }

		[JsonProperty]
		public int LifePoint { get; }

		[JsonProperty]
		public double WinPoint { get; }

		[JsonIgnore]
		public int Point => MatchPoint;

		public MatchResultTotal(IEnumerable<IResult> source) {
			if (source.Any()) {
				foreach (var data in source) {
					var point = data as IMatchResult;
					if (point != null) {
						MatchPoint += point?.MatchPoint ?? 0;
						LifePoint += point?.LifePoint ?? 0;
					}
				}
				WinPoint = (double)MatchPoint / source.Count();
			}
		}
	}

	internal class SingleMatchResult : IMatchResult {

		[JsonIgnore]
		public int MatchPoint
			=> ResultValue == RESULT.Win ? 3 : ResultValue == RESULT.Lose ? 0 : 1;

		[JsonIgnore]
		public double WinPoint
			=> ResultValue == RESULT.Win ? 1.0 : ResultValue == RESULT.Lose ? 0.0 : 0.5;

		[JsonProperty]
		public int LifePoint { get; }

		[JsonProperty]
		private RESULT ResultValue { get; }

		[JsonIgnore]
		public int Point => MatchPoint;

		public SingleMatchResult(RESULT result, int lifePoint = 0) {
			ResultValue = result;
			LifePoint = lifePoint;
		}
	}

	[JsonObject]
	public interface IMatchResult : IResult {
		int MatchPoint { get; }

		int LifePoint { get; }

		double WinPoint { get; }
	}
}