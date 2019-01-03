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
		public Result[] Results { get; private set; }

		[JsonProperty]
		public bool IsGapMatch { get; }

		[JsonIgnore]
		public bool IsFinished
			=> !Results.Any(result => result.Point == null);

		[JsonIgnore]
		public bool IsByeMatch
			=> Results.Any(result => result.Player == BYE);

		public void SetPoint(Params x, Params y) {
			Results[0].SetPoint(x);
			Results[1].SetPoint(y);
		}

		public void Commit()
			=> Results.ForEach(result => result.Player.Commit(this));

		private Result GetPlayerResult(Player player)
			=> Results.First(r => r.Player == player);

		private Result GetOpponentResult(Player player)
			=> Results.First(r => r.Player != player);

		public Params GetPoint(Player player)
			=> GetPlayerResult(player)?.Point;

		public Player GetOpponent(Player player)
			=> GetOpponentResult(player)?.Player;

		public Params GetOpponentPoint(Player player)
			=> GetOpponentResult(player)?.Point;

		public int GetResult(Player player)
			=> GetPoint(player)?.CompareTo(GetOpponentPoint(player)) ?? 0;

		private Match() {// For Serializer
		}

		public Match(Player player1, Player player2) {
			Results = new[] { new Result(player1), new Result(player2) };
			IsGapMatch = (player1.Point.MatchPoint != player2.Point.MatchPoint);
		}

		public Match(Player player) {
			Results = new[] { new Result(player), new Result(BYE) };
		}
	}
}