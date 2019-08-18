using BSMM2.Models.Rules.Match;
using BSMM2.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

namespace BSMM2.Models {

	public class Application {

		private enum INITIAL_METHOD { BY_COUNT, BY_ENTRY };

		private static Rule[] _rules = { new MatchRule() };

		[JsonProperty]
		private INITIAL_METHOD InitialMethod { get; set; } = INITIAL_METHOD.BY_COUNT;

		[JsonProperty]
		public int PlayerCount { get; set; } = 8;

		[JsonProperty]
		public int TryCount { get; set; } = 100;

		[JsonProperty]
		public Rule Rule { get; set; } = _rules[0];

		[JsonProperty]
		public string PlayerNamePrefix { get; set; } = "Player";

		[JsonProperty]
		public List<string> Entries { get; set; }

		[JsonProperty]
		public int EntryId { get; set; }

		[JsonProperty]
		public string Filename { get; set; } = "temp";

		[JsonProperty]
		public Game Game { get; set; }

		public void Initial() {
			switch (InitialMethod) {
				case INITIAL_METHOD.BY_COUNT:
					Game = new Game(Rule, new Players(PlayerCount, PlayerNamePrefix));
					break;

				case INITIAL_METHOD.BY_ENTRY:
					Game = new Game(Rule, new Players(new StringReader(Entries[EntryId])));
					break;
			}
		}

		public void Load() {
			var file = IsolatedStorageFile.GetUserStoreForApplication();
			using (var strm = file.OpenFile(Filename + ".json", FileMode.Open))
			using (var reader = new StreamReader(strm)) {
				Game = new Serializer<Game>().Deserialize(reader);
			}
		}

		public void Save() {
			var file = IsolatedStorageFile.GetUserStoreForApplication();
			using (var strm = file.CreateFile(Filename + ".json"))
			using (var writer = new StreamWriter(strm)) {
				new Serializer<Game>().Serialize(writer, Game);
			}
		}
	}
}