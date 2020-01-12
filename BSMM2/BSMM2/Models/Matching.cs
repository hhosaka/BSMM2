using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	public class Matching : IRound {

		[JsonProperty]
		public Match[] Matches { get; private set; }

		public void Swap(int m1, int m2)
			=> Swap(Matches[m1], Matches[m2]);

		private Matching() {
		}

		public void Swap(Match m1, Match m2) {
			if (!m1.IsByeMatch && !m2.IsByeMatch)
				m1.Swap(m2);
		}

		public Matching(IEnumerable<Match> matches) {
			Matches = matches?.ToArray();
		}
	}
}