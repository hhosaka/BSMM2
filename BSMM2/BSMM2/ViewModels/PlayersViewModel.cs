using BSMM2.Models;
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

		public PlayersViewModel(BSMMApp app) {
			_app = app;
			Title = "Players";
			Players = new ObservableCollection<Player>();

			MessagingCenter.Subscribe<object>(this, "UpdatedRound",
				async (sender) => await Refresh());
			MessagingCenter.Subscribe<object>(this, "UpdatedMatch",
				async (sender) => await Refresh());
		}

		public async Task Refresh() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => Players = new ObservableCollection<Player>(Game.PlayersByOrder ?? Enumerable.Empty<Player>()));
				} finally {
					IsBusy = false;
				}
			}
		}
	}
}