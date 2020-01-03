using BSMM2.Models;
using BSMM2.Resource;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

		private string _timer;

		public string Timer {
			get => _timer;
			set => SetProperty(ref _timer, value);
		}

		private string _count;

		public string Count {
			get => _count;
			set => SetProperty(ref _count, value);
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
			Title = AppResource.RoundPageTitle;
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

		private async Task UpdateList() {
			await Task.Run(() => Matches = Game.ActiveRound?.Matches ?? Enumerable.Empty<Match>());
			Count = "Round" + (Game.Rounds?.Count() + 1 ?? 0);
		}

		private DelegateCommand CreateStepToPlayingCommand() {
			return new DelegateCommand(
				async () => await Invoke(Execute),
				() => Game.CanExecuteStepToPlaying());

			async Task Execute() {
				await Task.Run(() => Game.StepToPlaying());
				StartTimer();
			}
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