using BSMM2.Models;
using BSMM2.Models.Matches.SingleMatch;
using BSMM2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
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
			game.ActiveRound.Swap(0, 1);
			Util.Check(new[] { 3, 2, 1, 4, 5, 6 }, _origin, game.ActiveRound);
			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.Players.GetByOrder());

			var buf = new StringBuilder();

			new Serializer<Game>().Serialize(new StringWriter(buf), game);

			var sbuf = buf.ToString();

			var result = new Serializer<Game>().Deserialize(new StringReader(sbuf));

			Util.Check(new[] { 3, 2, 1, 4, 5, 6 }, _origin, result.ActiveRound);
			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, result.Players.GetByOrder());
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
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.Players.GetByOrder());

			var json = JsonConvert.SerializeObject(game, settings);
			var result = JsonConvert.DeserializeObject<Game>(json, settings);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, result.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, result.Players.GetByOrder());
		}

		[TestMethod]
		public void LoadSaveTest3() {
			var rule = new SingleMatchRule();
			var game = new FakeGame(rule, 6, _origin);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);

			game.StepToPlaying();

			game.ActiveRound.ElementAt(0).SetResults(rule.CreatePoints(Win));
			game.ActiveRound.ElementAt(1).SetResults(rule.CreatePoints(Win));
			game.ActiveRound.ElementAt(2).SetResults(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.Players.GetByOrder());

			var engine = new SerializeUtil();

			game.Save(engine);

			var game2 = Game.Load(game.Id, engine);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game2.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game2.Players.GetByOrder());
			Assert.AreEqual(0, game2.Rounds.Count());
		}

		[TestMethod]
		public void LoadSaveTest5() {
			var rule = new SingleMatchRule();
			var src = new FakeGame(rule, 8, _origin);

			src.StepToPlaying();

			src.ActiveRound.ElementAt(0).SetResults(rule.CreatePoints(Win));
			src.ActiveRound.ElementAt(1).SetResults(rule.CreatePoints(Win));
			src.ActiveRound.ElementAt(2).SetResults(rule.CreatePoints(Win));
			src.ActiveRound.ElementAt(3).SetResults(rule.CreatePoints(Win));

			var engine = new SerializeUtil();

			src.Save(engine);
			var dst = Game.Load(src.Id, engine);

			Assert.AreEqual(0, dst.Rounds.Count());

			dst.StepToMatching();

			Assert.AreEqual(1, dst.Rounds.Count());

			dst.StepToPlaying();
			dst.ActiveRound.ElementAt(0).SetResults(rule.CreatePoints(Win));

			Assert.AreEqual(1, dst.Rounds.Count());

			dst.ActiveRound.ElementAt(1).SetResults(rule.CreatePoints(Win));
			dst.ActiveRound.ElementAt(2).SetResults(rule.CreatePoints(Win));
			dst.ActiveRound.ElementAt(3).SetResults(rule.CreatePoints(Win));

			Assert.AreEqual(1, dst.Rounds.Count());

			dst.StepToMatching();

			Assert.AreEqual(2, dst.Rounds.Count());

			dst.StepToPlaying();

			Assert.AreEqual(2, dst.Rounds.Count());
		}

		[TestMethod]
		public void LoadSaveTest6() {
			var app = BSMMApp.Create("test.json", true);

			app.Game.StepToPlaying();

			Assert.AreEqual(0, app.Game.Rounds.Count());

			app.Game.ActiveRound.ElementAt(0).SetResults(app.Game.Rule.CreatePoints(Win));
			app.Game.ActiveRound.ElementAt(1).SetResults(app.Game.Rule.CreatePoints(Win));
			app.Game.ActiveRound.ElementAt(2).SetResults(app.Game.Rule.CreatePoints(Win));
			app.Game.ActiveRound.ElementAt(3).SetResults(app.Game.Rule.CreatePoints(Win));

			app.Game.StepToMatching();

			Assert.AreEqual(1, app.Game.Rounds.Count());
			Assert.AreEqual(1, app.Games.Count());

			app.Save(true);

			Assert.AreEqual(1, app.Game.Rounds.Count());
			Assert.AreEqual(1, app.Games.Count());

			var dst = BSMMApp.Create("test.json", false);

			Assert.AreEqual(1, app.Games.Count());
			Assert.AreEqual(1, dst.Game.Rounds.Count());
		}

		private class Sample {
			public string Title { get; set; }
			public List<string> List { get; set; }
			public List<Round> Rounds { get; set; }
			public List<Game> Games { get; set; }

			public Sample() {
				List = new List<string>();
				Rounds = new List<Round>();
				Games = new List<Game>();
			}
		}

		[TestMethod]
		public void LoadSaveTest4() {
			var src = new Sample();
			src.Title = "test";
			src.List.Add("item1");
			src.Rounds.Add(new Round());
			src.Rounds.Add(new Round());
			src.Games.Add(new Game());
			src.Games.Add(new Game());

			var buf = new StringBuilder();

			new Serializer<Sample>().Serialize(new StringWriter(buf), src);

			var dst = new Serializer<Sample>().Deserialize(new StringReader(buf.ToString()));

			Assert.AreEqual("test", dst.Title);
			CollectionAssert.AreEqual(new[] { "item1" }, dst.List);
			Assert.AreEqual(2, dst.Rounds.Count);
			Assert.AreEqual(2, dst.Games.Count);
		}

		[TestMethod]
		public void LoadSaveAppTest1() {
			var buf = new StringBuilder();

			var app = BSMMApp.Create(TESTFILE, false);
			app.Rule = app.Rules.ElementAt(1);
			new Serializer<BSMMApp>().Serialize(new StringWriter(buf), app);

			var sbuf = buf.ToString();

			var result = new Serializer<BSMMApp>().Deserialize(new StringReader(sbuf));
			Assert.IsTrue(result.Rules.Count() == 3);
			Assert.AreEqual(result.Rule, result.Rules.ElementAt(1));
		}

		private const string TESTFILE = "testdata.json";

		[TestMethod]
		public void LoadSaveAppTest2() {
			var app = BSMMApp.Create(TESTFILE, true);
			app.Rule = app.Rules.ElementAt(0);
			app.Save(true);
			var app2 = BSMMApp.Create(TESTFILE, false);
			Assert.IsTrue(app2.Rules.Count() == 3);
			Assert.AreEqual(app2.Rules.ElementAt(0), app2.Rule);
			Assert.AreEqual(1, app2.Games.Count());
			Assert.AreEqual(app.Games.Last(), app.Game);
		}

		[TestMethod]
		public void LoadSaveAppTest3() {
			var app = BSMMApp.Create(TESTFILE, true);
			var rule = app.Rules.ElementAt(1);
			app.Rule = rule;
			var title = "test";
			app.Add(new Game(rule, new Players(app.Rule, 8), title), true);
			app.Save(true);
			var app2 = BSMMApp.Create(TESTFILE, false);
			Assert.IsTrue(app2.Rules.Count() == 3);
			Assert.AreEqual(app2.Rules.ElementAt(1), app2.Rule);
			Assert.AreEqual(1, app2.Games.Count());
			Assert.AreEqual(app.Game.Id, app2.Game.Id);
		}
	}
}