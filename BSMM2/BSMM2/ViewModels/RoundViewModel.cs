using BSMM2.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class RoundViewModel : BaseViewModel {

		public class MatchItem : INotifyPropertyChanged, IMatch {
			private Match _match;

			public MatchItem(Match match) {
				_match = match;
			}

			public void SetResults((IResult, IResult) points) {
				_match.SetResults(points);
				PropertyChanged(this, new PropertyChangedEventArgs("Records"));
			}

			public IEnumerable<IMatchRecord> Records => _match.Records;

			public event PropertyChangedEventHandler PropertyChanged;
		}

		private BSMMApp _app;
		public Game Game => _app.Game;
		private IEnumerable<MatchItem> _matches;

		public IEnumerable<MatchItem> Matches {
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

			MessagingCenter.Subscribe<object>(this, "RefreshGame",
				async (sender) => await Refresh(false));

			MessagingCenter.Subscribe<object>(this, "UpdateMatch",
				(sender) => RaiseCanExecuteChanged());
		}

		public bool IsPlaying
			=> Game?.IsMatching() == false;

		private void Execute(Action action) {
			if (!IsBusy) {
				IsBusy = true;
				try {
					action();
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

		private DelegateCommand CreateStepToPlayingCommand() {
			return new DelegateCommand(
				() => Execute(Game.StepToPlaying),
				() => Game.CanExecuteStepToPlaying());
		}

		private DelegateCommand CreateStepToMatchingCommand() {
			return new DelegateCommand(
				async () => await Execute(),
				() => Game.CanExecuteStepToMatching());

			async Task Execute() {
				if (!IsBusy) {
					IsBusy = true;

					try {
						if (Game.StepToMatching()) {
							RaiseCanExecuteChanged();
							Matches = new List<MatchItem>(Game.ActiveRound.Matches.Select(m => new MatchItem(m)));
						} else {
							await OnMatchingFailed?.Invoke();
						}
					} finally {
						IsBusy = false;
					}
				}
			}
		}

		private DelegateCommand CreateShuffleCommand() {
			return new DelegateCommand(async () => await Refresh(true), () => Game.CanExecuteShuffle() == true);
		}

		private async Task Refresh(bool doShuffle) {
			if (!IsBusy) {
				IsBusy = true;

				try {
					if (doShuffle) Game.Shuffle();
					await UpdateList();
					RaiseCanExecuteChanged();
				} finally {
					IsBusy = false;
				}
			}
		}

		private async Task UpdateList()
			=> await Task.Run(() => Matches = new List<MatchItem>(Game.ActiveRound.Matches.Select(m => new MatchItem(m))));
	}
}