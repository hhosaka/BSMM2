﻿using BSMM2.Models;
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

		public Game Game => _app.Game;

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
			=> !Game.ActiveRound.IsPlaying;

		public DelegateCommand ShuffleCommand { get; }
		public DelegateCommand StartCommand { get; }
		public DelegateCommand StepToMatchingCommand { get; }
		public DelegateCommand ShowRoundsLogCommand { get; }

		public event Func<Task> OnFailedMatching;

		public RoundViewModel(BSMMApp app, Action showRoundsLog) {
			Debug.Assert(app != null);
			_app = app;
			ShuffleCommand = CreateShuffleCommand();
			StartCommand = CreateStepToPlayingCommand();
			StepToMatchingCommand = CreateStepToMatchingCommand();
			ShowRoundsLogCommand = new DelegateCommand(showRoundsLog, () => app.Game.Rounds.Any());
			MessagingCenter.Subscribe<object>(this, Messages.REFRESH,
				async (sender) => await ExecuteRefresh());

			StartTimer();
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
			IsTimerVisible = (Game.StartTime != null);

			Matches = Game.ActiveRound;
			Title = Game.Headline;
			StartCommand?.RaiseCanExecuteChanged();
			ShuffleCommand?.RaiseCanExecuteChanged();
			StepToMatchingCommand?.RaiseCanExecuteChanged();
			ShowRoundsLogCommand?.RaiseCanExecuteChanged();
		}

		private DelegateCommand CreateStepToPlayingCommand() {
			return new DelegateCommand(
				Execute,
				() => Game.CanExecuteStepToPlaying());

			async void Execute() {
				Game.StepToPlaying();
				StartTimer();
				await ExecuteRefresh();
			}
		}

		private DelegateCommand CreateShuffleCommand() {
			return new DelegateCommand(
				Execute,
				() => Game.CanExecuteShuffle());

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
				() => Game.CanExecuteStepToMatching());

			async void Execute() {
				if (Game.StepToMatching()) {
					await ExecuteRefresh();
				} else {
					await OnFailedMatching();
				}
			}
		}

		private void StartTimer() {
			Device.StartTimer(TimeSpan.FromMilliseconds(100), () => {
				if (Game.StartTime == null) {
					IsTimerVisible = false;
				} else {
					Device.BeginInvokeOnMainThread(() => {
						Timer = (DateTime.Now - Game.StartTime)?.ToString(@"hh\:mm\:ss");
					});
				}
				return true;
			});
		}
	}
}