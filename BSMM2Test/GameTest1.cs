using BSMM2.Models;
using BSMM2.Models.Rules.Match;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

			Util.Check(new[] { 1, 2, 3, 4 }, game.OrderedPlayers);

			game.Players.Add("Player006");
			Util.Check(new[] { 1, 2, 3, 4, 6 }, game.OrderedPlayers);

			game.Players.Add("Player005");
			Util.Check(new[] { 1, 2, 3, 4, 6, 5 }, game.OrderedPlayers);

			game.Players.Remove(1);
			Util.Check(new[] { 1, 3, 4, 6, 5 }, game.OrderedPlayers);

			game.Players.Add();
			Util.Check(new[] { 1, 3, 4, 6, 5, 6 }, game.OrderedPlayers);
		}

		[TestMethod]
		public void GameInitiateByListTest() {
			var buf = "\r\nPlayer001\r\nPlayer002\r\n\r\nPlayer003\r\nPlayer004";
			var rule = new MatchRule();
			var game = new FakeGame(rule, new StringReader(buf));

			Util.Check(new[] { 1, 2, 3, 4 }, game.OrderedPlayers);

			game.Players.Add("Player006");
			Util.Check(new[] { 1, 2, 3, 4, 6 }, game.OrderedPlayers);
		}

		[TestMethod]
		public void GameSequence1Test() {
			var rule = new MatchRule();
			var game = new FakeGame(rule, 4);

			game.Shuffle();

			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);
			Util.Check(new[] { 1, 2, 3, 4 }, game.OrderedPlayers);

			game.StepToPlaying();

			Util.Check(new[] { 1, 2, 3, 4 }, game.ActiveRound);
			Util.Check(new[] { 1, 2, 3, 4 }, game.OrderedPlayers);

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 2, 3, 4 }, game.OrderedPlayers);

			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			Util.Check(new[] { 1, 3, 2, 4 }, game.OrderedPlayers);

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Lose));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Lose));

			Util.Check(new[] { 2, 4, 1, 3 }, game.OrderedPlayers);

			game.StepToMatching();
			game.OrderedPlayers.ToArray()[0].Drop();

			Util.Check(new[] { 4, 1, 3, 2 }, game.OrderedPlayers);

			game.Shuffle();

			Util.Check(new[] { 4, 1, 3, 2 }, game.OrderedPlayers);
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

			(game.ActiveRound as Matching)?.Swap(game.ActiveRound.Matches[0], game.ActiveRound.Matches[1]);
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
			Util.Check(new[] { 1, 3, 2, 4 }, game.OrderedPlayers);
			Util.Check(new[] { 1, 3, 2, 4 }, game.ActiveRound);
			Assert.AreEqual(1, game.Rounds.Count());
			Util.Check(new[] { 1, 2, 3, 4 }, game.Rounds.First());

			game.StepToPlaying();

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Lose));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Lose));

			Assert.IsTrue(game.CanExecuteStepToMatching());
			Util.Check(new[] { 3, 1, 4, 2 }, game.OrderedPlayers);
		}

		[TestMethod]
		public void OrderTestSingleMatch1()
			=> OrderTest1(new MatchRule());

		[TestMethod]
		public void OrderTestSingleMatch2()
			=> OrderTest2(new MatchRule());

		[TestMethod]
		public void OrderTestMultiMatch1()
			=> OrderTest1(new MultiMatchRule(2, 0));

		[TestMethod]
		public void OrderTestMultiMatch2()
			=> OrderTest2(new MultiMatchRule(2, 0));

		[TestMethod]
		public void OrderTestSingleMatch3()
			=> Util.Check(new[] { 1, 5, 2, 6, 3, 7, 4, 8 }, CreateGame(new MatchRule(), 8, 3).OrderedPlayers);

		[TestMethod]
		public void OrderTestSingleMatch4()
			=> Util.Check(new[] { 1, 5, 2, 6, 3, 7, 4 }, CreateGame(new MatchRule(), 7, 3).OrderedPlayers);

		[TestMethod]
		public void ライフポイント検証() {
			var rule = new MatchRule();
			var game = CreateGame(rule, 8, 2);
			var matches = game.ActiveRound.Matches;

			Util.Check(new[] { 1, 3, 5, 7, 2, 4, 6, 8 }, game.ActiveRound);
			Util.Check(new[] { 1, 5, 2, 3, 6, 7, 4, 8 }, game.OrderedPlayers);

			matches[0].SetPoint(rule.CreatePoints((Win, 4, 5)));

			Util.Check(new[] { 5, 1, 6, 7, 2, 3, 4, 8 }, game.OrderedPlayers);
		}

		[TestMethod]
		public void 勝利ポイント検証() {
			var rule = new MultiMatchRule(2, 0);
			var game = CreateGame(rule, 8, 2);
			var matches = game.ActiveRound.Matches;

			Util.Check(new[] { 1, 3, 5, 7, 2, 4, 6, 8 }, game.ActiveRound);
			Util.Check(new[] { 1, 5, 2, 3, 6, 7, 4, 8 }, game.OrderedPlayers);

			matches[0].SetPoint(rule.CreatePoints(new[] { Win, Lose, Win }));

			Util.Check(new[] { 5, 1, 3, 6, 7, 2, 4, 8 }, game.OrderedPlayers);
		}

		//
		// 8人対戦
		//
		private void OrderTest1(Rule rule) {
			var game = new FakeGame(rule, 8);

			// 初期設定確認
			Util.Check(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, game.ActiveRound);

			game.StepToPlaying();

			// 対戦開始時
			Util.Check(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, game.OrderedPlayers);

			// 3 win 4 lose
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));

			Util.Check(new[] { 3, 4, 1, 2, 5, 6, 7, 8 }, game.OrderedPlayers);

			// 1 win 2 lose
			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			Util.Check(new[] { 1, 3, 2, 4, 5, 6, 7, 8 }, game.OrderedPlayers);

			// 5  6 draw
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Draw));
			Util.Check(new[] { 1, 3, 5, 6, 2, 4, 7, 8 }, game.OrderedPlayers);

			// 8 win 7 lose
			game.ActiveRound.Matches[3].SetPoint(rule.CreatePoints(Lose));

			Util.Check(new[] { 1, 3, 8, 5, 6, 2, 4, 7 }, game.OrderedPlayers);

			// 7 win 8 lose
			game.ActiveRound.Matches[3].SetPoint(rule.CreatePoints(Win));
			Util.Check(new[] { 1, 3, 7, 5, 6, 2, 4, 8 }, game.OrderedPlayers);

			// 7 win 8 lose
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));
			Util.Check(new[] { 1, 3, 5, 7, 2, 4, 6, 8 }, game.OrderedPlayers);

			// 2回戦目
			game.StepToMatching();
			game.StepToPlaying();

			// 7 win 8 lose
			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Lose));
			game.StepToMatching();//無効であることを確認
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			game.StepToMatching();//無効であることを確認
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));
			game.StepToMatching();//無効であることを確認
			game.ActiveRound.Matches[3].SetPoint(rule.CreatePoints(Win));
			Util.Check(new[] { 5, 3, 1, 6, 7, 2, 4, 8 }, game.OrderedPlayers);

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			Util.Check(new[] { 1, 5, 2, 3, 6, 7, 4, 8 }, game.OrderedPlayers);

			// 3回戦目
			game.StepToMatching();
			game.StepToPlaying();

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[3].SetPoint(rule.CreatePoints(Win));
			Util.Check(new[] { 1, 5, 2, 6, 3, 7, 4, 8 }, game.OrderedPlayers);
		}

		//
		// 7人対戦
		//
		private void OrderTest2(Rule rule) {
			var game = new FakeGame(rule, 7);

			// 初期設定確認
			Util.Check(new[] { 1, 2, 3, 4, 5, 6, 7, -1 }, game.ActiveRound);

			game.StepToPlaying();

			// 対戦開始時
			Util.Check(new[] { 7, 1, 2, 3, 4, 5, 6 }, game.OrderedPlayers);

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 3, 5, 7, 2, 4, 6 }, game.OrderedPlayers);

			game.StepToMatching();
			game.StepToPlaying();

			Util.Check(new[] { 1, 3, 5, 7, 2, 4, 6, -1 }, game.ActiveRound);

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 5, 2, 3, 6, 7, 4 }, game.OrderedPlayers);

			game.StepToMatching();
			game.StepToPlaying();

			Util.Check(new[] { 1, 5, 2, 3, 6, 7, 4, -1 }, game.ActiveRound);

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));

			var points = game.OrderedPlayers.Select(p => p.Result.Point);
			var opponentPoints = game.OrderedPlayers.Select(p => p.OpponentResult.Point);

			Util.Check(new[] { 1, 5, 2, 6, 3, 7, 4 }, game.OrderedPlayers);
		}

		private Game CreateGame(Rule rule, int count, int round) {
			var game = new FakeGame(rule, count);

			for (int i = 0; i < round; ++i) {
				game.StepToMatching();
				game.StepToPlaying();
				game.ActiveRound.Matches.ForEach(m => m.SetPoint(rule.CreatePoints(Win)));
			}
			return game;
		}
	}
}