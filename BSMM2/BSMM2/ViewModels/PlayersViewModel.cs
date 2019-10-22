using BSMM2.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class PlayersViewModel : BaseViewModel {
		private Game _game;

		private IEnumerable<Player> _players;

		public IEnumerable<Player> Players {
			get => _players;
			set { SetProperty(ref _players, value); }
		}

		public PlayersViewModel() {
			Title = "Players";
			Players = new ObservableCollection<Player>();

			MessagingCenter.Subscribe<object, Game>(this, "RefreshGame", async (sender, game) => {
				await ExecuteLoadPlayersCommand(game);
			});
		}

		private async Task ExecuteLoadPlayersCommand(Game game) {
			if (!IsBusy) {
				IsBusy = true;

				try {
					if (game != null)
						_game = game;

					await Task.Run(() => Players = _game.PlayersByOrder);
				} finally {
					IsBusy = false;
				}
			}
		}
	}
}