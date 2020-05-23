using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	public class Round : List<Match> {
		public bool IsPlaying { get; set; }

		public bool IsFinished
			=> !this.Any(match => !match.IsFinished);

		public void Swap(int m1, int m2)
			=> Swap(this.ElementAt(m1), this.ElementAt(m2));

		public void Swap(Match m1, Match m2) {
			if (!m1.IsByeMatch && !m2.IsByeMatch)
				m1.Swap(m2);
		}

		public void Commit() {
			IsPlaying = true;
			ForEach(match => match.Commit());
		}

		public Round() {
		}

		public Round(IEnumerable<Match> matches) : base(matches) {
		}
	}
}