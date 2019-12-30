using BSMM2.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class PlayersViewModel : BaseViewModel {
		private BSMMApp _app;
		private Game Game => _app.Game;

		private IEnumerable<Player> _players;

		public IEnumerable<Player> Players {
			get => _players;
			set { SetProperty(ref _players, value); }
		}

		public PlayersViewModel(BSMMApp app) {
			_app = app;
			Title = "Players";
			Players = new ObservableCollection<Player>();

			MessagingCenter.Subscribe<object>(this, "RefreshGame",
				async (sender) => await Refresh());
			MessagingCenter.Subscribe<object>(this, "UpdateMatch",
				async (sender) => await Refresh());
		}

		private async Task Refresh() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => Players = Game?.PlayersByOrder);
				} finally {
					IsBusy = false;
				}
			}
		}
	}
}