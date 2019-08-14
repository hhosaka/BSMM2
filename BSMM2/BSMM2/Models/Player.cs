using BSMM2.Modules.Rules;
using BSMM2.Modules.Rules.Match;
using BSMM2.Services;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	[JsonObject(nameof(Player))]
	public class Player : IComparable<Player> {

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public bool Dropped { get; private set; }

		[JsonProperty]
		public IList<Match> Matches { get; private set; }

		[JsonIgnore]
		public Result Result//???
			=> Result.Sum(Matches.Select(match => match.GetPoint(this)));

		[JsonIgnore]
		public bool HasByeMatch
			=> Matches.Any(match => match.IsByeMatch);

		[JsonIgnore]
		public bool HasGapMatch
			=> Matches.Any(match => match.IsGapMatch);

		public int? GetResult(Player player)
			=> Matches.FirstOrDefault(m => m.GetOpponent(this) == player)?.GetResult(this);

		public void Commit(Match match)
			=> Matches.Add(match);

		public void Drop()
			=> Dropped = true;

		public int CompareTo(Player other) {
			if (this == other) {
				return 0;
			} else if (Dropped) {
				return other.Dropped ? 0 : -1;
			} else if (other.Dropped) {
				return 1;
			}
			return 0;
			//} else {
			//	return Result.CompareTo(other.Result);
			//}
		}

		private Player() {// For Serializer
		}

		public Player(string name) {
			Matches = new List<Match>();
			Name = name;
		}
	}
}