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
		private IsolatedStorageFile _store;

		public Game Load(Guid id) {
			using (var strm = _store.OpenFile(id.ToString() + ".json", FileMode.Open))
			using (var reader = new StreamReader(strm)) {
				return new Serializer<Game>().Deserialize(reader);
			}
		}

		public void Save(Game game) {
			using (var strm = _store.CreateFile(game.Id.ToString() + ".json"))
			using (var writer = new StreamWriter(strm)) {
				new Serializer<Game>().Serialize(writer, game);
			}
		}

		public void Remove(Game game) {
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