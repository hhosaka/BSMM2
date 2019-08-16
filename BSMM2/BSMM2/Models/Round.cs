using BSMM2.Modules.Rules;
using BSMM2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSMM2.Models {

	[JsonObject]
	public class Round {

		[JsonProperty]
		public Match[] Matches { get; private set; }

		[JsonIgnore]
		public bool IsFinished
			=> !Matches.Any(match => !match.IsFinished);

		public Round() {
		}

		public Round(IEnumerable<Match> matches) {
			Matches = matches.ToArray();
			Matches.ToList().ForEach(match => match.Commit());
		}
	}
}