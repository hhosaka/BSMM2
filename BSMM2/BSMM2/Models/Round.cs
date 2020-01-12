using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	public class Round : List<Match>, IRound {

		public bool IsFinished
			=> !this.Any(match => !match.IsFinished);

		public Round() {
		}

		public Round(IEnumerable<Match> matches) : base(matches) {
			ForEach(match => match.Commit());
		}
	}
}