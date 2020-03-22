using BSMM2.Models;
using BSMM2.Models.Matches.SingleMatch;
using BSMM2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using static BSMM2.Models.RESULT_T;

namespace BSMM2Test {

	[TestClass]
	public class SerializeTest {
		private readonly string _origin = "debug";

		private readonly JsonSerializerSettings settings
			= new JsonSerializerSettings {
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Auto
			};

		[TestMethod]
		public void LoadSaveTest1() {
			var game = new FakeGame(new SingleMatchRule(), 6, _origin);

			game.Shuffle();
			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);
			(game.ActiveRound as Matching)?.Swap(0, 1);
			Util.Check(new[] { 3, 2, 1, 4, 5, 6 }, _origin, game.ActiveRound);

			var buf = new StringBuilder();

			new Serializer<Game>().Serialize(new StringWriter(buf), game);

			var sbuf = buf.ToString();

			var result = new Serializer<Game>().Deserialize(new StringReader(sbuf));

			Util.Check(new[] { 3, 2, 1, 4, 5, 6 }, _origin, result.ActiveRound);
			Assert.AreEqual(game.Title, result.Title);
			Assert.AreEqual(game.Id, result.Id);
			Util.Check(game, result);
		}

		[TestMethod]
		public void LoadSaveTest2() {
			var rule = new SingleMatchRule();
			var game = new FakeGame(rule, 6, _origin);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);

			game.StepToPlaying();

			game.ActiveRound.ElementAt(0).SetResults(rule.CreatePoints(Win));
			game.ActiveRound.ElementAt(1).SetResults(rule.CreatePoints(Win));
			game.ActiveRound.ElementAt(2).SetResults(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.PlayersByOrder);

			var json = JsonConvert.SerializeObject(game, settings);
			var result = JsonConvert.DeserializeObject<Game>(json, settings);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.PlayersByOrder);
		}

		[TestMethod]
		public void LoadSaveTest3() {
			var filename = "debug";
			var rule = new SingleMatchRule();
			var game = new FakeGame(rule, 6, _origin);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);

			game.StepToPlaying();

			game.ActiveRound.ElementAt(0).SetResults(rule.CreatePoints(Win));
			game.ActiveRound.ElementAt(1).SetResults(rule.CreatePoints(Win));
			game.ActiveRound.ElementAt(2).SetResults(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.PlayersByOrder);

			var buf = new StringBuilder();

			new Serializer<Game>().Serialize(new StringWriter(buf), game);

			var sbuf = buf.ToString();

			var engine = new Engine();

			engine.SaveGame(game);

			var game2 = engine.LoadGame(game.Id);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game2.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game2.PlayersByOrder);
		}

		[TestMethod]
		public void LoadSaveAppTest1() {
			var buf = new StringBuilder();

			var app = BSMMApp.Create();
			app.Rule = app.Rules.ElementAt(1);
			new Serializer<BSMMApp>().Serialize(new StringWriter(buf), app);

			var sbuf = buf.ToString();

			var result = new Serializer<BSMMApp>().Deserialize(new StringReader(sbuf));
			Assert.IsTrue(result.Rules.Count() == 3);
			Assert.AreEqual(result.Rule, result.Rules.ElementAt(1));
		}

		[TestMethod]
		public void LoadSaveAppTest2() {
			var app = BSMMApp.Create();
			app.Rule = app.Rules.ElementAt(0);
			app.Save(true);
			var app2 = new Engine().CreateApp();
			Assert.IsTrue(app2.Rules.Count() == 3);
			Assert.AreEqual(app2.Rules.ElementAt(0), app2.Rule);
			Assert.AreEqual(0, app2.Games.Count());
			Assert.AreEqual(Guid.Empty, app.Game.Id);
		}

		[TestMethod]
		public void LoadSaveAppTest3() {
			var app = BSMMApp.Create();
			app.Rule = app.Rules.ElementAt(1);
			var title = "test";
			app.Add(new Game(app.Rule, new Players(8), title), true);
			app.Save(true);
			var app2 = new Engine().CreateApp();
			Assert.IsTrue(app2.Rules.Count() == 3);
			Assert.AreEqual(app2.Rules.ElementAt(1), app2.Rule);
			Assert.AreEqual(1, app2.Games.Count());
			Assert.AreEqual(app.Game.Id, app2.Game.Id);
		}

		[TestMethod]
		public void SaveSettings() {
			var entries = new[] { "123\r\n456", "abc\r\ndef" };
			var playerName = "test";
			var count = 123;

			var settings = new Settings() {
				Count = count,
				PlayerNamePrefix = playerName,
				Entries = entries,
				Rule = new SingleMatchRule()
			};

			var serializer = new Serializer<Settings>();
			var buf = new StringBuilder();

			serializer.Serialize(new StringWriter(buf), settings);
			var result = serializer.Deserialize(new StringReader(buf.ToString()));

			Assert.AreEqual(count, result.Count);
			Assert.AreEqual(playerName, result.PlayerNamePrefix);
			CollectionAssert.AreEqual(entries.ToArray(), result.Entries.ToArray());
			Assert.IsNotNull(result.Rule as Rule);
		}
	}
}