using BSMM2.Models;
using BSMM2.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class RoundViewModel : BaseViewModel {
		public ObservableCollection<Match> Matches { get; set; }
		public Command LoadRoundCommand { get; set; }
		public Command ShuffleCommand { get; set; }
		public Command StartCommand { get; set; }
		public Command NextRoundCommand { get; set; }

		public RoundViewModel() {
			Title = "Players";
			Matches = new ObservableCollection<Match>();
			LoadRoundCommand = new Command(async () => await ExecuteLoadRoundCommand());
			ShuffleCommand = new Command(async () => await ExecuteShuffleCommand());
			StartCommand = new Command(async () => await ExecuteShuffleCommand());
			NextRoundCommand = new Command(async () => await ExecuteShuffleCommand());

			MessagingCenter.Subscribe<NewGamePage>(this, "NewGame", async obj => {
				await ExecuteLoadRoundCommand();
			});
			MessagingCenter.Subscribe<NewGamePage>(this, "Shuffle", async obj => {
				await ExecuteShuffleCommand();
			});
		}

		private async Task Load() {
			Matches.Clear();
			foreach (var match in BSMMApp.Instance.Game.ActiveRound.Matches) {
				await Task.Run(() => Matches.Add(match));
			}
		}

		private async Task ExecuteShuffleCommand() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => BSMMApp.Instance.Game.Shuffle());
					await Load();
				} catch (Exception ex) {
					Debug.WriteLine(ex);
				} finally {
					IsBusy = false;
				}
			}
		}

		private async Task ExecuteLoadRoundCommand() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Load();
				} catch (Exception ex) {
					Debug.WriteLine(ex);
				} finally {
					IsBusy = false;
				}
			}
		}
	}
}