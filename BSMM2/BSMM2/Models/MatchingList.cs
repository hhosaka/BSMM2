using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSMM2.Models {

	public class MatchingList {

		[JsonProperty]
		public Match[] Matches { get; private set; }

		[JsonProperty]
		public bool Locked { get; set; }

		public void Swap(int m1, int m2) {
			Swap(Matches[m1], Matches[m2]);
		}

		public void Swap(Match m1, Match m2) {
			if (!Locked)
				m1.Swap(m2);
		}

		public MatchingList(IEnumerable<Match> matches) {
			Matches = matches.ToArray();
		}
	}
}