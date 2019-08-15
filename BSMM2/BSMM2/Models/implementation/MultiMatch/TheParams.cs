using BSMM2.Models;
using BSMM2.Modules.Rules.Match;
using BSMM2.Modules.Rules.SingleMatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.Rule;

namespace BSMM2.Modules.Rules.MultiMatch {

	[JsonObject]
	public class MultiMutciResult : IMatchResult {

		[JsonProperty]
		private IEnumerable<IMatchResult> _paramsList;

		[JsonIgnore]
		public int MatchPoint
			=> _paramsList.Sum(p => p.MatchPoint);

		[JsonIgnore]
		public int LifePoint
			=> _paramsList.Sum(p => p.LifePoint);

		[JsonIgnore]
		public double WinPoint
			=> _paramsList.Sum(p => p.WinPoint);

		public int Point
			=> MatchPoint;

		public MultiMutciResult(IEnumerable<IMatchResult> paramsList) {//TODO : tentative
			_paramsList = paramsList;
		}
	}
}