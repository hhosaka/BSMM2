using BSMM2.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class RoundViewModel : BaseViewModel {
		public ObservableCollection<Match> Matches { get; set; }
		public Command LoadMatchesCommand { get; set; }

		public RoundViewModel() {
			Title = "Players";
			Matches = new ObservableCollection<Match>();
			LoadMatchesCommand = new Command(async () => await ExecuteLoadMatchesCommand());

			//MessagingCenter.Subscribe<NewGamePage>(this, "NewGame", async obj => {
			//	await ExecuteLoadPlayersCommand();
			//});
		}

		private async Task ExecuteLoadMatchesCommand() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					Matches.Clear();
					foreach (var match in BSMMApp.Instance.Game.ActiveRound.Matches) {
						await Task.Run(() => Matches.Add(match));
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