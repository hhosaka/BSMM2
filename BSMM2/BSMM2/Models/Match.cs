using BSMM2.Modules.Rules;
using BSMM2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	[JsonObject]
	public class Match {
		public static readonly Player BYE = new Player("BYE");

		[JsonProperty]
		public Score[] Results { get; private set; }

		[JsonProperty]
		public bool IsGapMatch { get; }

		[JsonIgnore]
		public bool IsFinished
			=> !Results.Any(result => result.Point == null);

		[JsonIgnore]
		public bool IsByeMatch
			=> Results.Any(result => result.Player == BYE);

		public void SetPoint(Point x, Point y) {
			Results[0].SetPoint(x);
			Results[1].SetPoint(y);
		}

		public void Commit() {
			Results.ForEach(result => result.Player.Matches.Add(this));
		}

		private Score GetPlayerResult(Player player)
			=> Results.First(r => r.Player == player);

		private Score GetOpponentResult(Player player)
			=> Results.First(r => r.Player != player);

		public Point GetPoint(Player player)
			=> GetPlayerResult(player)?.Point;

		public Player GetOpponent(Player player)
			=> GetOpponentResult(player)?.Player;

		public Point GetOpponentPoint(Player player)
			=> GetOpponentResult(player)?.Point;

		public int GetResult(Player player)
			=> GetPoint(player)?.CompareTo(GetOpponentPoint(player)) ?? 0;

		public Match() {
		}

		public Match(Rule rule, Player player1, Player player2) {
			Results = new[] { new Score(player2), new Score(player1) };
			IsGapMatch = rule.CreateComparer().Compare(player1, player2) != 0;
		}

		public Match(Player player) {
			Results = new[] { new Score(player), new Score(BYE) };
		}
	}
}