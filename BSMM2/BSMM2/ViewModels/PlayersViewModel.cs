using BSMM2.Models;
using BSMM2.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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

			MessagingCenter.Subscribe<NewGamePage>(this, "NewGame", async obj => {
				await ExecuteLoadPlayersCommand();
			});
		}

		private async Task ExecuteLoadPlayersCommand() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					Players.Clear();
					foreach (var player in BSMMApp.Instance.Game.PlayersByOrder) {
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