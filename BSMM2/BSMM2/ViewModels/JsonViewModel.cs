using BSMM2.Models;
using BSMM2.Services;
using Prism.Commands;
using System;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class JsonViewModel : BaseViewModel {
		public string Buf { get; set; }
		public bool AsCurrentGame { get; set; }
		public DelegateCommand ImportCommand { get; }
		public DelegateCommand SendByMailCommand { get; }

		public JsonViewModel(BSMMApp app, Action Close) {
			var buf = new StringBuilder();
			new Serializer<IGame>().Serialize(new StringWriter(buf), app.Game);
			Buf = buf.ToString();
			AsCurrentGame = true;

			ImportCommand = new DelegateCommand(Import);
			SendByMailCommand = new DelegateCommand(SendByMail);

			void Import() {
				app.Add(new Serializer<Game>().Deserialize(new StringReader(Buf)), AsCurrentGame);
				MessagingCenter.Send<object>(this, Messages.REFRESH);
				Close?.Invoke();
			}

			void SendByMail() {
				app.SendByMail(app.Game.Headline, Buf);
			}
		}
	}
}