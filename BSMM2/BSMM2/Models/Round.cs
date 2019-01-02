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
		private readonly Match[] _matches;

		[JsonProperty]
		public bool Lock { get; set; }

		[JsonIgnore]
		public bool IsFinished
			=> !_matches.Any(match => !match.IsFinished);

		public Match[] Matches => _matches;

		public void Swap(int m1, int m2) {
			Swap(_matches.ElementAt(m1), _matches.ElementAt(m2));
		}

		public void Swap(Match m1, Match m2) {
			var temp = m1.Results[0];
			m1.Results[0] = m2.Results[0];
			m2.Results[0] = temp;
		}

		public void Commit() {
			foreach (var match in _matches) {
				Lock = true;
				match.Commit();
			}
		}

		public Round(IEnumerable<Match> matches) {
			_matches = matches.ToArray();
			Lock = false;
		}
	}
}