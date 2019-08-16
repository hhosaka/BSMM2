using BSMM2.Modules.Rules;
using BSMM2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSMM2.Models {

	[JsonObject]
	public class Round : IRound {

		[JsonProperty]
		public Match[] Matches { get; private set; }

		[JsonIgnore]
		public bool IsFinished
			=> !Matches.Any(match => !match.IsFinished);

		[JsonIgnore]
		public bool Locked {
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public void Swap(int m1, int m2)
			=> throw new NotImplementedException();

		public void Swap(Match m1, Match m2)
			=> throw new NotImplementedException();

		public Round() {
		}

		public Round(IEnumerable<Match> matches) {
			Matches = matches.ToArray();
			Matches.ToList().ForEach(match => match.Commit());
		}
	}
}