using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject(nameof(Player))]
	public class Player : IComparable<Player> {

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public bool Dropped { get; private set; }

		[JsonProperty]
		private IList<Match> _matches;

		[JsonIgnore]
		private IResult _result;

		[JsonIgnore]
		public IResult Result => _result;

		[JsonIgnore]
		public bool HasByeMatch
			=> _matches.Any(match => match.IsByeMatch);

		[JsonIgnore]
		public bool HasGapMatch
			=> _matches.Any(match => match.IsGapMatch);

		public RESULT? GetResult(Player player)
			=> _matches.FirstOrDefault(m => m.GetOpponentPlayer(this) == player)?.GetResult(this)?.RESULT;

		public void Commit(Match match)
			=> _matches.Add(match);

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
		}

		public void Reset(Rule rule)
			=> _result = rule.Sum(_matches.Select(match => match.GetResult(this)));

		public Player() {// For Serializer
		}

		public Player(string name) {
			_matches = new List<Match>();
			Name = name;
		}
	}
}