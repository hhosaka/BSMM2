using BSMM2.Modules.Rules;
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
	public class Player {

		[JsonProperty]
		public string Name { get; set; }

		[JsonProperty]
		public bool Dropped { get; set; }

		[JsonProperty]
		public IList<Match> Matches { get; set; }

		[JsonIgnore]
		public IEnumerable<Point> Points
			=> Matches.Select(match => match.GetPoint(this));

		[JsonIgnore]
		public IEnumerable<Point> OpponentPoints
			=> Matches.Select(match => match.GetOpponentPoint(this));

		[JsonIgnore]
		public bool HasByeMatch
			=> Matches.Any(match => match.IsByeMatch);

		[JsonIgnore]
		public bool HasGapMatch
			=> Matches.Any(match => match.IsGapMatch);

		public int? HasMatched(Player player)
			=> Matches.FirstOrDefault(m => m.GetOpponent(this) == player)?.GetResult(this);

		public void Drop()
			=> Dropped = true;

		public Player() {
		}

		public Player(string name) {
			Matches = new List<Match>();
			Name = name;
		}
	}
}