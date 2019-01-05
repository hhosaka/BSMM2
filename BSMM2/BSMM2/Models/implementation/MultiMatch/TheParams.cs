using BSMM2.Models;
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
	public class MultiMutciResult : Result {

		[JsonProperty]
		private IEnumerable<Result> _paramsList;

		[JsonIgnore]
		public override int MatchPoint
			=> _paramsList.Sum(p => p.MatchPoint);

		[JsonIgnore]
		public override int LifePoint
			=> _paramsList.Sum(p => p.LifePoint);

		[JsonIgnore]
		public override double WinPoint
			=> _paramsList.Sum(p => p.WinPoint);

		public MultiMutciResult(IEnumerable<Result> paramsList) {
			_paramsList = paramsList;
		}
	}
}