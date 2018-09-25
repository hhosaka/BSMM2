using BSMM2.Models;
using BSMM2.Services.implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

            matches[0].SetPoint(new SingleMatchPoint(3));
            matches[1].SetPoint(new SingleMatchPoint(3));

            var result = players.OrderBy(player => player.Point, rule.Comparer);
            Assert.IsTrue(result.ToArray() == players);
        }
    }
}
