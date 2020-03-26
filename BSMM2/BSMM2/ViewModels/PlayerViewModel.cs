using BSMM2.Models;

namespace BSMM2.ViewModels {

	public class PlayerViewModel : BaseViewModel {
		private BSMMApp _app;
		public Player Player { get; }

		public PlayerViewModel(BSMMApp app, Player player) {
			_app = app;
			Player = player;
		}
	}
}