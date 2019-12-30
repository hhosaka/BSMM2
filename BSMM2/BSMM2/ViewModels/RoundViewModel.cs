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
		public DelegateCommand NextRoundCommand { get; }

		public RoundViewModel(BSMMApp app) {
			_app = app;
			Title = "Round";
			ShuffleCommand = new DelegateCommand(async () => { Game?.Shuffle(); await Refresh(); }, () => Game?.CanExecuteShuffle() == true);
			StartCommand = new DelegateCommand(async () => await Playing(), () => Game?.CanExecuteStepToPlaying() == true);
			NextRoundCommand = new DelegateCommand(async () => await NextRound(), () => Game?.CanExecuteStepToMatching() == true);

			MessagingCenter.Subscribe<object>(this, "RefreshGame",
				async (sender) => await Refresh());

			MessagingCenter.Subscribe<object>(this, "UpdateMatch",
				(sender) => RaiseCanExecuteChanged());
		}

		public bool IsPlaying
			=> Game?.IsMatching() == false;

		public bool IsMatching
			=> Game?.IsMatching() == true;

		private void RaiseCanExecuteChanged() {
			StartCommand.RaiseCanExecuteChanged();
			ShuffleCommand.RaiseCanExecuteChanged();
			NextRoundCommand.RaiseCanExecuteChanged();
		}

		private async Task NextRound() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => Game.StepToMatching());
					RaiseCanExecuteChanged();
				} catch (Exception ex) {
					Debug.WriteLine(ex);
				} finally {
					IsBusy = false;
				}
			}
		}

		private async Task Playing() {
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

		private async Task Refresh() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					var list = Game?.ActiveRound?.Matches;
					await Task.Run(() => Matches = list == null ? null : new List<MatchItem>(list.Select(m => new MatchItem(m))));
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