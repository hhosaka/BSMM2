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

		[JsonObject]
		private class Record {

			[JsonProperty]
			public Player Player { get; }

			[JsonProperty]
			public Result Point { get; private set; }

			public void SetPoint(Result point)
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

		public void SetPoint(Result x, Result y) {
			Results[0].SetPoint(x);
			Results[1].SetPoint(y);
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

		public Result GetPoint(Player player)
			=> GetPlayerResult(player)?.Point;

		public Player GetOpponent(Player player)
			=> GetOpponentResult(player)?.Player;

		public Result GetOpponentPoint(Player player)
			=> GetOpponentResult(player)?.Point;

		public int GetResult(Player player)
			=> GetPoint(player)?.CompareTo(GetOpponentPoint(player)) ?? 0;

		private Match() {// For Serializer
		}

		public Match(Player player1, Player player2) {
			Results = new[] { new Record(player1), new Record(player2) };
			IsGapMatch = (player1.Point.MatchPoint != player2.Point.MatchPoint);
		}

		public Match(Player player) {
			Results = new[] { new Record(player), new Record(BYE) };
		}
	}
}