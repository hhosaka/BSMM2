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

		[JsonProperty]
		public bool Locked { get; private set; }

		[JsonIgnore]
		public bool IsFinished
			=> !Matches.Any(match => !match.IsFinished);

		public void Lock()
			=> Locked = true;

		public void Unlock()
			=> Locked = false;

		public void Swap(int m1, int m2) {
			Swap(Matches[m1], Matches[m2]);
		}

		public void Swap(Match m1, Match m2) {
			m1.Swap(m2);
		}

		public void Commit() {
			Locked = true;
			Matches.ToList().ForEach(match => match.Commit());
		}

		public Round(IEnumerable<Match> matches) {
			Matches = matches.ToArray();
		}
	}
}