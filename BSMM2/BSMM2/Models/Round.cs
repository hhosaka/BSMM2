using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject]
	public class Round {

		[JsonProperty]
		public List<Match> Matches { get; private set; }

		[JsonProperty]
		public bool IsPlaying { get; private set; }

		[JsonIgnore]
		public bool IsFinished
			=> !Matches.Any(match => !match.IsFinished);

		public bool Swap(int m1, int m2)
			=> Swap(Matches.ElementAt(m1), Matches.ElementAt(m2));

		public bool Swap(Match m1, Match m2) {
			if (!IsPlaying && !m1.IsByeMatch && !m2.IsByeMatch) {
				m1.Swap(m2);
				return true;
			}
			return false;
		}

		public void Commit() {
			IsPlaying = true;
			Matches.ForEach(match => match.Commit());
		}

		public Round() {
		}

		public Round(IEnumerable<Match> matches) {
			Matches = new List<Match>(matches);
		}
	}
}