using BSMM2.Models;
using BSMM2.Services.implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace BSMM2Test {

	[TestClass]
	public class BSMM2Test {

		private int ConvId(Player player) {
			if (player.Name == "BYE") {
				return -1;
			} else {
				return int.Parse(player.Name.Substring("Player".Length));
			}
		}

		private void Check(IEnumerable<int> expect, IEnumerable<Player> players) {
			Assert.IsTrue(expect.SequenceEqual(players.Select(ConvId)));
		}

		private void Check(IEnumerable<int> expect, Round round) {
			Assert.IsTrue(expect.Reverse().SequenceEqual(
				round.Matches.SelectMany(match => match.Results.Select(result => ConvId(result.Player)))));
		}

		[TestMethod]
		public void RuleTest() {
			var rule = new SingleMatchRule();
			var players = new[] { new Player("player1"), new Player("player2"), new Player("player3"), new Player("player4") };
			var matches = new[] { new Match(rule, players[0], players[1]), new Match(rule, players[2], players[3]) };

			Assert.IsTrue(players.SequenceEqual(players.OrderByDescending(player => player, rule.CreateComparer())));

			matches.ForEach(match => match.Commit());

			Assert.IsTrue(players.SequenceEqual(players.OrderByDescending(p => p, rule.CreateComparer())));

			matches[0].SetPoint(new SingleMatchRule.ThePoint(3), new SingleMatchRule.ThePoint(0));
			matches[1].SetPoint(new SingleMatchRule.ThePoint(3), new SingleMatchRule.ThePoint(0));

			Assert.IsTrue(new[] { players[0], players[2], players[1], players[3] }
				.SequenceEqual(players.OrderByDescending(p => p, rule.CreateComparer())));

			matches[0].SetPoint(new SingleMatchRule.ThePoint(0), new SingleMatchRule.ThePoint(3));
			matches[1].SetPoint(new SingleMatchRule.ThePoint(0), new SingleMatchRule.ThePoint(3));

			Assert.IsTrue(new[] { players[1], players[3], players[0], players[2] }
				.SequenceEqual(players.OrderByDescending(p => p, rule.CreateComparer())));
		}

		[TestMethod]
		public void GameAddPlayerTest() {
			var game = new Game(new SingleMatchRule(), 4);
			Check(new[] { 1, 2, 3, 4 }, game.PlayerList);

			game.Add(new Player("Player006"));
			Check(new[] { 1, 2, 3, 4, 6 }, game.PlayerList);

			game.Add(new Player("Player005"));
			Check(new[] { 1, 2, 3, 4, 5, 6 }, game.PlayerList);
		}

		[TestMethod]
		public void GameGetPlayeresTest() {
			var game = new Game(new SingleMatchRule(), 4);

			game.Shuffle();

			Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);
		}
	}
}