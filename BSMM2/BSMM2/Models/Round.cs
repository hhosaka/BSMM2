using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject]
	public class Round : IRound {

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