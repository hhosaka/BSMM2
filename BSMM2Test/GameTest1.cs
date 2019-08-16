using BSMM2.Models;
using BSMM2.Models.Rules;
using BSMM2.Models.Rules.Match;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Xamarin.Forms.Internals;
using static BSMM2.Models.RESULT;

namespace BSMM2Test {

	[TestClass]
	public class BSMM2Test {

		[TestMethod]
		public void RuleTest() {
			var rule = new MatchRule();
			var players = new[] { new Player("player1"), new Player("player2"), new Player("player3"), new Player("player4") };
			var matches = new[] { new Match(players[0], players[1]), new Match(players[2], players[3]) };

			Assert.IsTrue(players.SequenceEqual(players.OrderByDescending(player => player, rule.CreateComparer())));

			matches.ForEach(match => match.Commit());

			CollectionAssert.Equals(players, players.OrderByDescending(p => p, rule.CreateComparer()));

			matches[0].SetPoint(rule.CreatePoints(Win));
			matches[1].SetPoint(rule.CreatePoints(Win));

			CollectionAssert.Equals(new[] { players[0], players[2], players[1], players[3] },
				players.OrderByDescending(p => p, rule.CreateComparer()));

			matches[0].SetPoint(rule.CreatePoints(Lose));
			matches[1].SetPoint(rule.CreatePoints(Lose));

			CollectionAssert.Equals(new[] { players[1], players[3], players[0], players[2] },
				players.OrderByDescending(p => p, rule.CreateComparer()));
		}

		[TestMethod]
		public void GameAddPlayerTest() {
			var rule = new MatchRule();
			var game = new FakeGame(rule, 4);

			Util.Check(new[] { 1, 2, 3, 4 }, game.Players.Result(rule));

			game.Players.Add("Player006");
			Util.Check(new[] { 1, 2, 3, 4, 6 }, game.Players.Result(rule));

			game.Players.Add("Player005");
			Util.Check(new[] { 1, 2, 3, 4, 6, 5 }, game.Players.Result(rule));

			game.Players.Remove(1);
			Util.Check(new[] { 1, 3, 4, 6, 5 }, game.Players.Result(rule));

			game.Players.Add();
			Util.Check(new[] { 1, 3, 4, 6, 5, 6 }, game.Players.Result(rule));
		}

		[TestMethod]
		public void GameInitiateByListTest() {
			var buf = "\r\nPlayer001\r\nPlayer002\r\n\r\nPlayer003\r\nPlayer004";
			var rule = new MatchRule();
			var game = new FakeGame(rule, new StringReader(buf));

			Util.Check(new[] { 1, 2, 3, 4 }, game.Players.Result(rule));

			game.Players.Add("Player006");
			Util.Check(new[] { 1, 2, 3, 4, 6 }, game.Players.Result(rule));
		}

		[TestMethod]
		public void GameSequence1Test() {
			var rule = new MatchRule();
			var game = new FakeGame(rule, 4);

			game.Shuffle();

			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);
			Util.Check(new[] { 1, 2, 3, 4 }, game.Players.Result(rule));

			game.StepToPlaying();

			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);
			Util.Check(new[] { 1, 2, 3, 4 }, game.Players.Result(rule));

			game.ActiveRound.Matches.ElementAt(0).SetPoint(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 2, 3, 4 }, game.Players.Result(rule));

			game.ActiveRound.Matches.ElementAt(1).SetPoint(rule.CreatePoints(Win));
			Util.Check(new[] { 1, 3, 2, 4 }, game.Players.Result(rule));

			game.ActiveRound.Matches.ElementAt(0).SetPoint(rule.CreatePoints(Lose));
			game.ActiveRound.Matches.ElementAt(1).SetPoint(rule.CreatePoints(Lose));

			Util.Check(new[] { 2, 4, 1, 3 }, game.Players.Result(rule));

			game.StepToMatching();
			game.Players.Result(rule).ElementAt(0).Drop();

			Util.Check(new[] { 4, 1, 3, 2 }, game.Players.Result(rule));

			game.Shuffle();

			Util.Check(new[] { 4, 1, 3, 2 }, game.Players.Result(rule));
			Util.Check(new[] { 4, 1, 3, -1 }, game.ActiveRound);
		}

		[TestMethod]
		public void GameSequence2Test() {
			var rule = new MatchRule();
			var game = new FakeGame(rule, 4);

			// 初期設定確認
			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);

			//　スワップできる
			Assert.IsFalse(game.Locked);
			(game.ActiveRound as Matching)?.Swap(0, 1);
			Util.Check(new[] { 3, 2, 1, 4 }, game.ActiveRound);

			//　シャッフルできる
			Assert.IsTrue(game.CanExecuteShuffle);
			game.Shuffle();
			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);

			(game.ActiveRound as Matching)?.Swap(game.ActiveRound.Matches.ElementAt(0), game.ActiveRound.Matches.ElementAt(1));
			Util.Check(new[] { 3, 2, 1, 4 }, game.ActiveRound);

			(game.ActiveRound as Matching)?.Swap(0, 1);
			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);

			//　ロックしてみる
			Assert.IsFalse(game.CanExecuteStepToMatching());
			Assert.IsTrue(game.CanExecuteStepToLock());
			Assert.IsTrue(game.CanExecuteStepToPlaying());
			Assert.IsFalse(game.CanExecuteBackToMatching());
			game.StepToLock();

			Assert.IsTrue(game.Locked);
			(game.ActiveRound as Matching)?.Swap(0, 1);
			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);
			Assert.IsTrue(game.Locked);

			game.Shuffle();
			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);
			Assert.IsTrue(game.Locked);

			// マッチングに戻る
			Assert.IsFalse(game.CanExecuteStepToMatching());
			Assert.IsFalse(game.CanExecuteStepToLock());
			Assert.IsTrue(game.CanExecuteStepToPlaying());
			Assert.IsTrue(game.CanExecuteBackToMatching());
			game.BackToMatching();

			Assert.IsFalse(game.Locked);
			game.Shuffle();
			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);

			// 試合中にする
			Assert.IsNull(game.ElapsedTime);
			game.StepToPlaying();
			Thread.Sleep(2);
			Assert.IsTrue(game.ElapsedTime?.Milliseconds > 0);

			Assert.IsFalse(game.CanExecuteStepToMatching());

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));

			Assert.IsFalse(game.CanExecuteStepToMatching());

			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));

			Assert.IsTrue(game.CanExecuteStepToMatching());

			game.StepToMatching();
			Util.Check(new[] { 1, 3, 2, 4 }, game.Players.Result(rule));
			Util.Check(new[] { 1, 3, 2, 4 }, game.ActiveRound);
			Assert.AreEqual(1, game.Rounds.Count());
			Util.Check(new[] { 1, 2, 3, 4 }, game.Rounds.First());

			game.StepToPlaying();

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Lose));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Lose));

			Assert.IsTrue(game.CanExecuteStepToMatching());
			Util.Check(new[] { 3, 1, 4, 2 }, game.Players.Result(rule));
		}
	}
}