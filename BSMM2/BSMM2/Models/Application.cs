using BSMM2.Models.Rules.Match;
using BSMM2.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BSMM2.Models {

	public class ApplicationXXX {

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
		public string Filename { get; set; } = "temp";

		[JsonProperty]
		public Game Game { get; set; }

		public void Initial(Rule rule, int count, string prefix) {
			InitialMethod = INITIAL_METHOD.BY_COUNT;
			Rule = rule;
			PlayerCount = count;
			PlayerNamePrefix = prefix;
			Game = new Game(rule, new Players(count, prefix));
		}

		public void Initial(Rule rule, string entry) {
			InitialMethod = INITIAL_METHOD.BY_ENTRY;
			Rule = rule;
			if (!Entries.Any(e => e == entry))
				Entries.Add(entry);

			Game = new Game(Rule, new Players(new StringReader(entry)));
		}

		private IsolatedStorageFile _store;

		private IsolatedStorageFile Store
			=> _store ?? (_store = IsolatedStorageFile.GetUserStoreForApplication());

		public IEnumerable<string> Filenames
			=> Store.GetFileNames();

		public void Load(string filename) {
			Filename = filename;
			using (var strm = Store.OpenFile(Filename + ".json", FileMode.Open))
			using (var reader = new StreamReader(strm)) {
				Game = new Serializer<Game>().Deserialize(reader);
			}
		}

		public void Save(string filename) {
			Filename = filename;
			using (var strm = Store.CreateFile(filename + ".json"))
			using (var writer = new StreamWriter(strm)) {
				new Serializer<Game>().Serialize(writer, Game);
			}
		}

		public async Task SendEmail(string subject, string body, List<string> recipients) {
			try {
				var message = new EmailMessage {
					Subject = subject,
					Body = body,
					To = recipients,
				};
				await Email.ComposeAsync(message);
			} catch (FeatureNotSupportedException) {
				// Email is not supported on this device
			}
		}

		public void SendByMail(string to, string filename) {
			var buf = new StringBuilder();
			using (var writer = new StringWriter(buf)) {
				new Serializer<Game>().Serialize(writer, Game);
				SendEmail("BS Match Maker Result", buf.ToString(), new[] { "hhosaka183@gmail.com" }.ToList()).Start();
			}
		}
	}
}