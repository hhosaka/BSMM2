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

		public IGame Game => _app.Game;

		private IEnumerable<Match> _matches;

		public IEnumerable<Match> Matches {
			get => _matches;
			set { SetProperty(ref _matches, value); }
		}

		private string _timer;

		public string Timer {
			get => _timer;
			set => SetProperty(ref _timer, value);
		}

		private bool _isTimeVisible;

		public bool IsTimerVisible {
			get => _isTimeVisible;
			set => SetProperty(ref _isTimeVisible, value);
		}

		public DelegateCommand ShuffleCommand { get; }
		public DelegateCommand StartCommand { get; }
		public DelegateCommand StepToMatchingCommand { get; }

		public event Func<Task> OnMatchingFailed;

		public RoundViewModel(BSMMApp app) {
			Debug.Assert(app != null);
			_app = app;
			ShuffleCommand = CreateShuffleCommand();
			StartCommand = CreateStepToPlayingCommand();
			StepToMatchingCommand = CreateStepToMatchingCommand();

			MessagingCenter.Subscribe<object>(this, Messages.REFRESH,
				async (sender) => await Invoke(() => Refresh()));

			Refresh();
		}

		public bool IsPlaying
			=> !Game.IsMatching;

		private async Task Invoke(Action action) {
			if (!IsBusy) {
				IsBusy = true;
				try {
					await Task.Run(action);
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

		private void Refresh() {
			Matches = Game.ActiveRound;
			Title = Game.Headline;
			RaiseCanExecuteChanged();
		}

		private DelegateCommand CreateStepToPlayingCommand() {
			return new DelegateCommand(
				async () => await Invoke(Execute),
				() => Game.CanExecuteStepToPlaying);

			void Execute() {
				Game.StepToPlaying();
				StartTimer();
			}
		}

		private DelegateCommand CreateShuffleCommand() {
			return new DelegateCommand(
				async () => await Invoke(Execute),
				() => Game.CanExecuteShuffle);

			void Execute() {
				if (Game.Shuffle()) {
					Refresh();
				} else {
					OnMatchingFailed();
				}
			}
		}

		private DelegateCommand CreateStepToMatchingCommand() {
			return new DelegateCommand(
				async () => await Invoke(Execute),
				() => Game.CanExecuteStepToMatching);

			void Execute() {
				if (Game.StepToMatching()) {
					Refresh();
				} else {
					OnMatchingFailed();
				}
			}
		}

		private void StartTimer() {
			IsTimerVisible = true;
			Device.StartTimer(TimeSpan.FromMilliseconds(100), () => {
				if (Game.StartTime == null) {
					IsTimerVisible = false;
					return false;
				} else {
					Timer = (DateTime.Now - Game.StartTime)?.ToString(@"hh\:mm\:ss");
					return true;
				}
			});
		}
	}
}