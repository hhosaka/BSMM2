using BSMM2.Models;
using BSMM2.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class PlayersViewModel : BaseViewModel {
		public ObservableCollection<Player> Players { get; set; }
		public Command LoadPlayersCommand { get; set; }

		public PlayersViewModel() {
			Title = "Players";
			Players = new ObservableCollection<Player>();
			LoadPlayersCommand = new Command(async () => await ExecuteLoadPlayersCommand());

			MessagingCenter.Subscribe<NewGamePage, Game>(this, "NewGame", async (obj, game) => {
				var newGame = game as Game;
				// BSMMApp.instance.Add(game);
				// Players.AddRange(game.Players);// TODO : TBD
				//await DataStore.AddItemAsync(newItem);
			});
		}

		private async Task ExecuteLoadPlayersCommand() {
			if (IsBusy)
				return;

			IsBusy = true;

			try {
				Players.Clear();
				var players = Enumerable.Empty<Player>();// TODO  await DataStore.GetItemsAsync(true);
				foreach (var player in players) {
					Players.Add(player);
				}
			} catch (Exception ex) {
				Debug.WriteLine(ex);
			} finally {
				IsBusy = false;
			}
		}
	}
}