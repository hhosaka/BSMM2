using BSMM2.Models;
using BSMM2.Models.Rules.Match;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Modules.Rules.MultiMatch {

	[JsonObject]
	public class MultiMatchResult : IMatchResult {

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

		[JsonIgnore]
		public RESULT? RESULT => throw new NotImplementedException();

		public int Point
			=> MatchPoint;

		public MultiMatchResult(IEnumerable<IMatchResult> paramsList) {//TODO : tentative
			_paramsList = paramsList;
		}
	}
}