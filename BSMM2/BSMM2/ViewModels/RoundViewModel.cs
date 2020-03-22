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

		public bool IsPlaying
			=> !Game.IsMatching;

		public DelegateCommand ShuffleCommand { get; }
		public DelegateCommand StartCommand { get; }
		public DelegateCommand StepToMatchingCommand { get; }

		public event Func<Task> OnFailedMatching;

		public RoundViewModel(BSMMApp app) {
			Debug.Assert(app != null);
			_app = app;
			ShuffleCommand = CreateShuffleCommand();
			StartCommand = CreateStepToPlayingCommand();
			StepToMatchingCommand = CreateStepToMatchingCommand();

			MessagingCenter.Subscribe<object>(this, Messages.REFRESH,
				async (sender) => await ExecuteRefresh());

			Refresh();
		}

		private async Task ExecuteRefresh() {
			if (!IsBusy) {
				IsBusy = true;
				try {
					await Task.Run(() => Refresh());
				} finally {
					IsBusy = false;
				}
			}
		}

		private void Refresh() {
			Matches = Game.ActiveRound;
			Title = Game.Headline;
			StartCommand?.RaiseCanExecuteChanged();
			ShuffleCommand?.RaiseCanExecuteChanged();
			StepToMatchingCommand?.RaiseCanExecuteChanged();
		}

		private DelegateCommand CreateStepToPlayingCommand() {
			return new DelegateCommand(
				Execute,
				() => Game.CanExecuteStepToPlaying);

			async void Execute() {
				Game.StepToPlaying();
				StartTimer();
				await ExecuteRefresh();
			}
		}

		private DelegateCommand CreateShuffleCommand() {
			return new DelegateCommand(
				Execute,
				() => Game.CanExecuteShuffle);

			async void Execute() {
				if (Game.Shuffle()) {
					await ExecuteRefresh();
				} else {
					await OnFailedMatching();
				}
			}
		}

		private DelegateCommand CreateStepToMatchingCommand() {
			return new DelegateCommand(
				Execute,
				() => Game.CanExecuteStepToMatching);

			async void Execute() {
				if (Game.StepToMatching()) {
					await ExecuteRefresh();
				} else {
					await OnFailedMatching();
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