using BSMM2.Models;
using BSMM2.Views;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class RoundViewModel : BaseViewModel {
		private Game _Game;

		public Game Game {
			get => _Game;
			private set {
				if (_Game != value) {
					_Game = value;
					OnPropertyChanged("Game");
				}
			}
		}

		public ObservableCollection<Match> Matches { get; set; }
		public Command LoadRoundCommand { get; set; }
		public DelegateCommand ShuffleCommand { get; set; }
		public Command StartCommand { get; set; }
		public Command NextRoundCommand { get; set; }

		public RoundViewModel() {
			Title = "Players";
			Game = BSMMApp.Instance.Game;
			Matches = new ObservableCollection<Match>();
			LoadRoundCommand = new Command(async () => await ExecuteLoadRoundCommand());
			ShuffleCommand = new DelegateCommand(async () => await ExecuteShuffleCommand(), () => Game?.CanExecuteShuffle() ?? false);
			StartCommand = new Command(async () => await ExecuteShuffleCommand(), () => Game?.CanExecuteStepToPlaying() ?? false);
			NextRoundCommand = new Command(async () => await ExecuteShuffleCommand(), () => Game?.CanExecuteStepToMatching() ?? false);

			MessagingCenter.Subscribe<NewGamePage>(this, "NewGame", async obj => {
				await ExecuteLoadRoundCommand();
			});
		}

		private async Task Load() {
			Matches.Clear();
			foreach (var match in Game.ActiveRound.Matches) {
				await Task.Run(() => Matches.Add(match));
			}
		}

		private async Task ExecuteNextRoundCommand() {
			await Task.Run(() => Game.StepToMatching());
		}

		private async Task ExecuteStartCommand() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => Game.StepToPlaying());
				} catch (Exception ex) {
					Debug.WriteLine(ex);
				} finally {
					IsBusy = false;
				}
			}
		}

		private async Task ExecuteShuffleCommand() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => Game.Shuffle());
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
					Game = BSMMApp.Instance.Game;
					await Load();
					ShuffleCommand.RaiseCanExecuteChanged();
				} catch (Exception ex) {
					Debug.WriteLine(ex);
				} finally {
					IsBusy = false;
				}
			}
		}
	}
}