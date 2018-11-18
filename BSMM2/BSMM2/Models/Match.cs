using BSMM2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	public class Match {
		public static readonly Player BYE = new Player("BYE");

		private Result[] _results;

		public IEnumerable<Result> Results => _results;

		public bool IsGapMatch { get; }

		public bool HasMatch(Player player)
			=> _results.Any(result => result.Player == player);

		public bool IsByeMatch => _results.Any(result => result.Player == BYE);

		public void SetPoint(IPoint x, IPoint y) {
			_results[0].SetPoint(x);
			_results[1].SetPoint(y);
		}

		public void Commit() {
			_results.ForEach(result => result.Player.Matches.Add(this));
		}

		private Result GetPlayerResult(Player player)
			=> _results.First(r => r.Player == player);

		private Result GetOpponentResult(Player player)
			=> _results.First(r => r.Player != player);

		public IPoint GetPoint(Player player) => GetPlayerResult(player)?.Point;

		public Player GetOpponent(Player player) => GetOpponentResult(player)?.Player;

		public IPoint GetOpponentPoint(Player player) => GetOpponentResult(player)?.Point;

		public int GetResult(Player player) {
			return GetPoint(player)?.CompareTo(GetOpponentPoint(player)) ?? 0;
		}

		public Match(Rule rule, Player player1, Player player2) {
			_results = new[] { new Result(player1), new Result(player2) };
			IsGapMatch = rule.CreateComparer().Compare(player1, player2) != 0;
		}

		public Match(Player player) {
			_results = new[] { new Result(player), new Result(BYE) };
		}
	}
}