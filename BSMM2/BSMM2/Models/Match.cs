using BSMM2.Modules.Rules;
using BSMM2.Modules.Rules.Match;
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

		[JsonObject]
		private class Record {

			[JsonProperty]
			public Player Player { get; }

			[JsonProperty]
			public IMatchResult Point { get; private set; }

			public void SetPoint(IMatchResult point)
				=> Point = point;

			private Record() {// For Serializer
			}

			public Record(Player player) {
				Player = player;
			}
		}

		public static readonly Player BYE = new Player("BYE");

		[JsonProperty]
		private Record[] Results { get; set; }

		[JsonProperty]
		public bool IsGapMatch { get; }

		[JsonIgnore]
		public bool IsFinished
			=> !Results.Any(result => result.Point == null);

		[JsonIgnore]
		public bool IsByeMatch
			=> Results.Any(result => result.Player == BYE);

		[JsonIgnore]
		public IEnumerable<string> PlayerNames
			=> Results.Select(result => result.Player.Name);

		public void SetPoint((IMatchResult, IMatchResult) points) {
			Results[0].SetPoint(points.Item1);
			Results[1].SetPoint(points.Item2);
		}

		public void Swap(Match other) {
			var temp = Results[0];
			Results[0] = other.Results[0];
			other.Results[0] = temp;
		}

		public void Commit()
			=> Results.ForEach(result => result.Player.Commit(this));

		private Record GetPlayerResult(Player player)
			=> Results.First(r => r.Player == player);

		private Record GetOpponentResult(Player player)
			=> Results.First(r => r.Player != player);

		public IMatchResult GetPoint(Player player)
			=> GetPlayerResult(player)?.Point;

		public Player GetOpponent(Player player)
			=> GetOpponentResult(player)?.Player;

		public IMatchResult GetOpponentPoint(Player player)
			=> GetOpponentResult(player)?.Point;

		public RESULT? GetResult(Player player) {
			var point = GetPoint(player)?.Point;
			switch (point) {
				case 3:
					return RESULT.Win;

				case 0:
					return RESULT.Lose;

				case 1:
					return RESULT.Draw;

				default:
					return null;
			}
		}

		private Match() {// For Serializer
		}

		public Match(Player player1, Player player2) {
			Results = new[] { new Record(player1), new Record(player2) };
			IsGapMatch = (player1.Result?.Point != player2.Result?.Point);
		}

		public Match(Player player) {
			Results = new[] { new Record(player), new Record(BYE) };
		}
	}
}