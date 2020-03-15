using BSMM2.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class PlayersViewModel : BaseViewModel {
		private BSMMApp _app;
		private IGame Game => _app.Game;

		private IEnumerable<Player> _players;

		public IEnumerable<Player> Players {
			get => _players;
			set { SetProperty(ref _players, value); }
		}

		public DelegateCommand NewGameCommand { get; }
		public DelegateCommand RuleCommand { get; }
		public DelegateCommand SelectGameCommand { get; }
		public DelegateCommand AddPlayerCommand { get; }
		public DelegateCommand ExportCommand { get; }
		public DelegateCommand HelpCommand { get; }

		public PlayersViewModel(BSMMApp app, Action newGame = null, Action openRule = null, Action selectGame = null, Action addPlayer = null) {
			_app = app;
			Players = new ObservableCollection<Player>();

			NewGameCommand = new DelegateCommand(() => newGame?.Invoke());
			RuleCommand = new DelegateCommand(() => openRule?.Invoke(), () => _app.Game.CanAddPlayers);
			SelectGameCommand = new DelegateCommand(() => selectGame?.Invoke(), () => _app.Games.Any());
			AddPlayerCommand = new DelegateCommand(() => addPlayer?.Invoke(), () => _app.Game.CanAddPlayers);

			MessagingCenter.Subscribe<object>(this, "UpdatedRound",
				async (sender) => await Refresh());
			MessagingCenter.Subscribe<object>(this, "UpdatedMatch",
				async (sender) => await Refresh());
		}

		public async Task Refresh() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => Execute());
				} finally {
					IsBusy = false;
				}
			}

			void Execute() {
				Players = new ObservableCollection<Player>(Game.PlayersByOrder ?? Enumerable.Empty<Player>());
				Title = Game.Headline;
				RuleCommand?.RaiseCanExecuteChanged();
				SelectGameCommand?.RaiseCanExecuteChanged();
				AddPlayerCommand?.RaiseCanExecuteChanged();
			}
		}
	}
}