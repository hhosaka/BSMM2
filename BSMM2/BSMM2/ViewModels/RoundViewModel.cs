using BSMM2.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class RoundViewModel : BaseViewModel {
		private BSMMApp _app;
		private IEnumerable<Match> _matches;

		public Game Game => _app.Game;

		public IEnumerable<Match> Matches {
			get => _matches;
			set { SetProperty(ref _matches, value); }
		}

		public DelegateCommand ShuffleCommand { get; }
		public DelegateCommand StartCommand { get; }
		public DelegateCommand StepToMatchingCommand { get; }

		public event Func<Task> OnMatchingFailed;

		public RoundViewModel(BSMMApp app) {
			Debug.Assert(app != null);
			_app = app;
			Title = "Round";
			ShuffleCommand = CreateShuffleCommand();
			StartCommand = CreateStepToPlayingCommand();
			StepToMatchingCommand = CreateStepToMatchingCommand();

			MessagingCenter.Subscribe<object>(this, "UpdatedRound",
				async (sender) => await Invoke(() => UpdateList()));

			MessagingCenter.Subscribe<object>(this, "UpdatedMatch",
				(sender) => RaiseCanExecuteChanged());
		}

		public bool IsPlaying
			=> Game?.IsMatching() == false;

		private async Task Invoke(Func<Task> action) {
			if (!IsBusy) {
				IsBusy = true;
				try {
					await action();
					RaiseCanExecuteChanged();
				} finally {
					IsBusy = false;
				}
			}
		}

		private void RaiseCanExecuteChanged() {
			StartCommand.RaiseCanExecuteChanged();
			ShuffleCommand.RaiseCanExecuteChanged();
			StepToMatchingCommand.RaiseCanExecuteChanged();
		}

		private async Task UpdateList()
			=> await Task.Run(() => Matches = Game.ActiveRound.Matches);

		private DelegateCommand CreateStepToPlayingCommand() {
			return new DelegateCommand(
				async () => await Invoke(() => Task.Run(() => Game.StepToPlaying())),
				() => Game.CanExecuteStepToPlaying());
		}

		private DelegateCommand CreateShuffleCommand() {
			return new DelegateCommand(
				async () => await Invoke(Execute),
				() => Game.CanExecuteShuffle());

			async Task Execute() {
				if (Game.Shuffle()) {
					await UpdateList();
				} else {
					await OnMatchingFailed();
				}
			}
		}

		private DelegateCommand CreateStepToMatchingCommand() {
			return new DelegateCommand(
				async () => await Invoke(Execute),
				() => Game.CanExecuteStepToMatching());

			async Task Execute() {
				if (Game.StepToMatching()) {
					await UpdateList();
				} else {
					await OnMatchingFailed();
				}
			}
		}
	}
}