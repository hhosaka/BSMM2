using BSMM2.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class PlayersViewModel : BaseViewModel {
		public ObservableCollection<Player> Players { get; set; }
		public Command<Game> LoadPlayersCommand { get; set; }

		public PlayersViewModel() {
			Title = "Players";
			Players = new ObservableCollection<Player>();
			LoadPlayersCommand = new Command<Game>(async game => await ExecuteLoadPlayersCommand(game));

			MessagingCenter.Subscribe<NewGameViewModel, Game>(this, "NewGame", async (sender, game) => {
				await ExecuteLoadPlayersCommand(game);
			});
		}

		private async Task ExecuteLoadPlayersCommand(Game game) {
			if (!IsBusy) {
				IsBusy = true;

				try {
					Players.Clear();
					foreach (var player in game.PlayersByOrder) {
						await Task.Run(() => Players.Add(player));
					}
				} catch (Exception ex) {
					Debug.WriteLine(ex);
				} finally {
					IsBusy = false;
				}
			}
		}
	}
}