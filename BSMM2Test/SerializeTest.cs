using BSMM2.Models;
using BSMM2.Models.Rules.Match;
using BSMM2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;
using static BSMM2.Models.RESULT;

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
			var game = new FakeGame(new MatchRule(), 6, _origin);

			game.Shuffle();
			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound.Matches);
			(game.ActiveRound as Matching)?.Swap(0, 1);
			Util.Check(new[] { 3, 2, 1, 4, 5, 6 }, _origin, game.ActiveRound.Matches);

			var buf = new StringBuilder();

			new Serializer<Game>().Serialize(new StringWriter(buf), game);

			var result = new Serializer<Game>().Deserialize(new StringReader(buf.ToString()));

			Util.Check(new[] { 3, 2, 1, 4, 5, 6 }, _origin, result.ActiveRound.Matches);
		}

		[TestMethod]
		public void LoadSaveTest2() {
			var rule = new MatchRule();
			var game = new FakeGame(rule, 6, _origin);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound.Matches);

			game.StepToPlaying();

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound.Matches);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.Players.GetPlayersByOrder(rule));

			var json = JsonConvert.SerializeObject(game, settings);
			var result = JsonConvert.DeserializeObject<Game>(json, settings);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound.Matches);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.Players.GetPlayersByOrder(rule));
		}

		[TestMethod]
		public void LoadSaveTest3() {
			var rule = new MatchRule();
			var game = new FakeGame(rule, 6, _origin);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound.Matches);

			game.StepToPlaying();

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound.Matches);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.Players.GetPlayersByOrder(rule));

			var application = new Application();
			application.Game = game;

			application.Save();
			application.Game = null;
			application.Load();

			var game2 = application.Game;

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game2.ActiveRound.Matches);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game2.Players.GetPlayersByOrder(rule));
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
				Rule = new MatchRule()
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