using BSMM2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSMM2.Models {

	public class Round {
		private readonly IEnumerable<Match> _matches;

		public IEnumerable<Match> Matches => _matches;

		public Round(IEnumerable<Match> matches) {
			_matches = matches;
		}
	}
}