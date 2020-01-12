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

		public event Action SelectGame;

		public DelegateCommand SelectGameCommand { get; }

		public PlayersViewModel(BSMMApp app) {
			_app = app;
			Players = new ObservableCollection<Player>();

			SelectGameCommand = new DelegateCommand(() => SelectGame?.Invoke(), () => _app.Games.Any());

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
					SelectGameCommand?.RaiseCanExecuteChanged();
				} finally {
					IsBusy = false;
				}
			}

			void Execute() {
				Players = new ObservableCollection<Player>(Game.PlayersByOrder ?? Enumerable.Empty<Player>());
				Title = Game.Headline;
			}
		}
	}
}