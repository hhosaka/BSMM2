﻿using BSMM2.Models;
using BSMM2.Models.Matches.SingleMatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;
using static BSMM2.Models.RESULT_T;

namespace BSMM2Test {

	[TestClass]
	public class ExportTest {

		[TestMethod]
		public void ExportTitleTest() {
			var rule = new SingleMatchRule();
			var player = new Player(rule, "test");
			var buf = new StringBuilder();

			player.ExportTitle(new StringWriter(buf));
			Assert.AreEqual("Name, Dropped, Point, WinPoint, LifePoint", buf.ToString());
		}

		[TestMethod]
		public void ExportDataTest1() {
			var rule = new SingleMatchRule();
			var player = new Player(rule, "test");
			var buf = new StringBuilder();

			player.ExportData(new StringWriter(buf));
			Assert.AreEqual("\"test\", False, 0, 0, 0", buf.ToString());
		}

		[TestMethod]
		public void ExportDataTest2() {
			var rule = new SingleMatchRule();
			var game = new FakeGame(rule, 4);

			game.StepToPlaying();
			game.ActiveRound.Matches.ElementAt(0).SetResult(Win);
			game.ActiveRound.Matches.ElementAt(1).SetResult(Win);

			var players = game.Players.GetByOrder();
			Util.CheckWithOrder(new[] { 1, 3, 2, 4 }, new[] { 1, 1, 3, 3 }, players);

			var buf = new StringBuilder();

			players.ElementAt(0).ExportData(new StringWriter(buf));
			Assert.AreEqual("\"Player001\", False, 3, 1, 0", buf.ToString());

			buf.Clear();
			players.ElementAt(2).ExportData(new StringWriter(buf));
			Assert.AreEqual("\"Player002\", False, 0, 0, 0", buf.ToString());

			buf.Clear();
			game.Players.Export(new StringWriter(buf));

			Assert.AreEqual("Name, Dropped, Point, WinPoint, LifePoint\r\n" +
							"\"Player001\", False, 3, 1, 0\r\n" +
							"\"Player003\", False, 3, 1, 0\r\n" +
							"\"Player002\", False, 0, 0, 0\r\n" +
							"\"Player004\", False, 0, 0, 0\r\n",
							buf.ToString());
		}
	}
}