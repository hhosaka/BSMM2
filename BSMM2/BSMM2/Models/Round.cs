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
			Swap(Matches.ElementAt(m1), Matches.ElementAt(m2));
		}

		public void Swap(Match m1, Match m2) {
			var temp = m1.Results[0];
			m1.Results[0] = m2.Results[0];
			m2.Results[0] = temp;
		}

		public void Commit() {
			foreach (var match in Matches) {
				Locked = true;
				match.Commit();
			}
		}

		public Round(IEnumerable<Match> matches) {
			Matches = matches.ToArray();
		}
	}
}