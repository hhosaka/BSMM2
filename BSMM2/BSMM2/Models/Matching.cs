using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	public class Matching : List<Match>, IRound {

		public void Swap(int m1, int m2)
			=> Swap(this.ElementAt(m1), this.ElementAt(m2));

		public void Swap(Match m1, Match m2) {
			if (!m1.IsByeMatch && !m2.IsByeMatch)
				m1.Swap(m2);
		}

		public Matching(IEnumerable<Match> matches) : base(matches) {
		}
	}
}