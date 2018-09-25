using BSMM2.Models;
using BSMM2.Services.implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace BSMM2Test
{
    [TestClass]
    public class BSMM2Test
    {
        [TestMethod]
        public void RuleTest()
        {
            var rule = new SingleMatchRule();
            var players = new[] { new Player("player1"), new Player("player2"), new Player("player3"), new Player("player4") };
            var matches = new[] { new Match(players[0], players[1]), new Match(players[2], players[3]) };

            Assert.IsTrue(players.SequenceEqual(players.OrderByDescending(player => player.Point)));

            matches.ForEach(match => match.Commit());

            Assert.IsTrue(players.SequenceEqual(rule.GetPlayer(players, 0, false)));

            matches[0].SetPoint(new SingleMatchPoint(3));
            matches[1].SetPoint(new SingleMatchPoint(3));

            Assert.IsTrue(new[] { players[0], players[2], players[1], players[3] }
            .SequenceEqual(rule.GetPlayer(players, 0, false)));

            matches[0].SetPoint(new SingleMatchPoint(0));
            matches[1].SetPoint(new SingleMatchPoint(0));

            Assert.IsTrue(new[] { players[1], players[3], players[0], players[2] }
            .SequenceEqual(rule.GetPlayer(players, 0, false)));
        }

        [TestMethod]
        public void GameAddPlayerTest()
        {
            var game = new Game(new SingleMatchRule(), 4);
            Assert.IsTrue(new[] { "Player001", "Player002", "Player003", "Player004" }.SequenceEqual(
                game.GetPlayersByName().Select(p => p.Name)));

            game.Add(new Player("Player005"));
            Assert.IsTrue(new[] { "Player001", "Player002", "Player003", "Player004", "Player005" }.SequenceEqual(
                game.GetPlayersByName().Select(p => p.Name)));

            game.Add(new Player("Player004-2"));
            Assert.IsTrue(new[] { "Player001", "Player002", "Player003", "Player004", "Player004-2", "Player005" }.SequenceEqual(
                game.GetPlayersByName().Select(p => p.Name)));

            var round = game.GetRound();
        }

        [TestMethod]
        public void GameGetPlayeresTest()
        {
            var game = new Game(new SingleMatchRule(), 4);

            var round = game.GetRound(true);

            Assert.IsTrue(new[] { "Player001", "Player002", "Player003", "Player004" }.SequenceEqual(
                game.GetPlayersByName().Select(p => p.Name)));

            game.Add(new Player("Player005"));
            Assert.IsTrue(new[] { "Player001", "Player002", "Player003", "Player004", "Player005" }.SequenceEqual(
                game.GetPlayersByName().Select(p => p.Name)));

            game.Add(new Player("Player004-2"));
            Assert.IsTrue(new[] { "Player001", "Player002", "Player003", "Player004", "Player004-2", "Player005" }.SequenceEqual(
                game.GetPlayersByName().Select(p => p.Name)));
        }
    }
}