using BSMM2.Models;
using BSMM2.Services;
using BSMM2.Modules.Rules.SingleMatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Xamarin.Forms.Internals;
using static BSMM2.Models.Rule.RESULT;
using BSMM2.Modules.Rules;

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
			const string filename = "backup";
			var game = new FakeGame(new MatchRule(), 6, _origin);

			game.Shuffle();
			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);
			game.ActiveRound.Swap(0, 1);
			Util.Check(new[] { 3, 2, 1, 4, 5, 6 }, _origin, game.ActiveRound);

			new Serializer<Game>().Serialize(filename, game);

			var result = new Serializer<Game>().Deserialize(filename);

			Util.Check(new[] { 3, 2, 1, 4, 5, 6 }, _origin, result.ActiveRound);
		}

		[TestMethod]
		public void LoadSaveTest2() {
			var rule = new MatchRule();
			var game = new FakeGame(rule, 6, _origin);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);

			game.StepToPlaying();

			game.ActiveRound.Matches[0].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[1].SetPoint(rule.CreatePoints(Win));
			game.ActiveRound.Matches[2].SetPoint(rule.CreatePoints(Win));

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.Players.Result(rule));

			var json = JsonConvert.SerializeObject(game, settings);
			var result = JsonConvert.DeserializeObject<Game>(json, settings);

			Util.Check(new[] { 1, 2, 3, 4, 5, 6 }, _origin, game.ActiveRound);
			Util.Check(new[] { 1, 3, 5, 2, 4, 6 }, _origin, game.Players.Result(rule));
		}

		[TestMethod]
		public void SaveSettings() {
			var filename = "backup";
			var entries = new[] { "123\r\n456", "abc\r\ndef" };
			var playerName = "test";
			var count = 123;

			var settings = new Settings() {
				Count = count,
				PlayerNamePrefix = playerName,
				Entries = entries,
				Rule = new Rule()
			};

			var serializer = new Serializer<Settings>();

			serializer.Serialize(filename, settings);
			var result = serializer.Deserialize(filename);

			Assert.AreEqual(count, result.Count);
			Assert.AreEqual(playerName, result.PlayerNamePrefix);
			CollectionAssert.AreEqual(entries.ToArray(), result.Entries.ToArray());
			Assert.IsNotNull(result.Rule as Rule);
		}
	}
}