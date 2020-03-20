using BSMM2.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BSMM2.Models {

	public class Engine {
		private static readonly string APPDATAPATH = "appfile.data";

		private IsolatedStorageFile _store;

		public T Load<T>(string filename) where T : new() {
			using (var strm = _store.OpenFile(filename, FileMode.Open))
			using (var reader = new StreamReader(strm)) {
				return new Serializer<T>().Deserialize(reader);
			}
		}

		public void Save<T>(T data, string filename) {
			using (var strm = _store.CreateFile(filename))
			using (var writer = new StreamWriter(strm)) {
				new Serializer<T>().Serialize(writer, data);
			}
		}

		public BSMMApp CreateApp() {
			if (_store.FileExists(APPDATAPATH)) {
				return Load<BSMMApp>(APPDATAPATH);
			} else {
				return new BSMMApp(this);
			}
		}

		public void SaveApp(BSMMApp app) {
			Save<BSMMApp>(app, APPDATAPATH);
		}

		public Game LoadGame(Guid id) {
			return Load<Game>(id.ToString() + ".json");
		}

		public void SaveGame(IGame game) {
			Save<IGame>(game, game.Id.ToString() + ".json");
		}

		public void RemoveGame(IGame game) {
			_store.DeleteFile(game.Id.ToString() + ".json");
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

		public void SendByMail(Game game, string to, string filename) {
			var buf = new StringBuilder();
			using (var writer = new StringWriter(buf)) {
				new Serializer<Game>().Serialize(writer, game);
				SendEmail("BS Match Maker Result", buf.ToString(), new[] { "hhosaka183@gmail.com" }.ToList()).Start();
			}
		}

		public Engine() {
			_store = IsolatedStorageFile.GetUserStoreForApplication();
		}
	}
}