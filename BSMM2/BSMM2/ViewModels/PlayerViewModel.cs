using BSMM2.Models;

namespace BSMM2.ViewModels {

	public class PlayerViewModel : BaseViewModel {
		public Player Player { get; }

		public PlayerViewModel(BSMMApp app, Player player) {
			Player = player;
		}
	}
}