using BSMM2.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	public class Player {
		public string Name { get; set; }
		public bool Dropped { get; private set; }
		public IList<Match> Matches { get; } = new List<Match>();

		public IEnumerable<IPoint> Points => Matches.Select(match => match.GetPoint(this));

		public IEnumerable<IPoint> OpponentPoints => Matches.Select(match => match.GetOpponentPoint(this));

		public bool HasByeMatch => Matches.Any(match => match.IsByeMatch);

		public bool HasGapMatch => Matches.Any(match => match.IsGapMatch);

		public bool HasMatched(Player player) {
			return Matches.Any(match => match.HasMatch(player));
		}

		public int CompareTo(Player p) {
			return Matches.FirstOrDefault(m => m.GetOpponent(this) == p)?.GetResult(this) ?? 0;
		}

		public void Drop() {
			Dropped = true;
		}

		public Player(string name) {
			Name = name;
		}
	}
}