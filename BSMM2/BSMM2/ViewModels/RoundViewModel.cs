using BSMM2.Models;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class RoundViewModel : BaseViewModel {
		public Game Game { get; set; }
		private ObservableCollection<Match> _matches;

		public ObservableCollection<Match> Matches {
			get => _matches;
			set { SetProperty(ref _matches, value); }
		}

		public Command LoadRoundCommand { get; set; }
		public DelegateCommand ShuffleCommand { get; set; }
		public DelegateCommand StartCommand { get; set; }
		public DelegateCommand NextRoundCommand { get; set; }

		public RoundViewModel() {
			Title = "Players";
			Game = BSMMApp.Instance.Game;
			Matches = new ObservableCollection<Match>();
			LoadRoundCommand = new Command<Game>(async game => await ExecuteLoadRoundCommand(game));
			ShuffleCommand = new DelegateCommand(async () => await ExecuteShuffleCommand(), () => Game?.CanExecuteShuffle() == true);
			StartCommand = new DelegateCommand(async () => await ExecuteStartCommand(), () => Game?.CanExecuteStepToPlaying() == true);
			NextRoundCommand = new DelegateCommand(async () => await ExecuteNextRoundCommand(), () => Game?.CanExecuteStepToMatching() == true);

			MessagingCenter.Subscribe<object, Game>(this, "RefreshGame", async (sender, game) => {
				await ExecuteLoadRoundCommand(game);
			});
		}

		public bool IsPlaying
			=> Game?.IsMatching() == false;

		public bool IsMatching
			=> Game?.IsMatching() == true;

		private async Task Load() {
			Matches = new ObservableCollection<Match>(Game.ActiveRound.Matches);
			//Matches.Clear();
			//foreach (var match in Game.ActiveRound.Matches) {
			//	await Task.Run(() => Matches.Add(new MatchItem(match)));
			//}
		}

		private async Task ExecuteNextRoundCommand() {
			await Task.Run(() => Game.StepToMatching());
		}

		private async Task ExecuteStartCommand() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => Game.StepToPlaying());
					RaiseCanExecuteChanged();
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

		private void RaiseCanExecuteChanged() {
			StartCommand.RaiseCanExecuteChanged();
			ShuffleCommand.RaiseCanExecuteChanged();
			NextRoundCommand.RaiseCanExecuteChanged();
		}

		private async Task ExecuteLoadRoundCommand(Game game) {
			if (!IsBusy) {
				IsBusy = true;

				try {
					if (game != null)
						Game = game;
					else
						Game = BSMMApp.Instance.Game;

					await Load();
					RaiseCanExecuteChanged();
				} catch (Exception ex) {
					Debug.WriteLine(ex);
				} finally {
					IsBusy = false;
				}
			}
		}
	}
}