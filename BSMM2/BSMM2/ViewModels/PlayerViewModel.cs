using BSMM2.Models;

namespace BSMM2.ViewModels {

	public class PlayerViewModel : BaseViewModel {
		private BSMMApp _app;
		private Player _player;
		public Player Player => _player;

		public PlayerViewModel(BSMMApp app, Player player) {
			_app = app;
			_player = player;
		}
	}
}