using BSMM2.Models;
using System;

namespace BSMM2.ViewModels {

	public class AddPlayerViewModel : BaseViewModel {
		private BSMMApp _app;
		private IGame Game => _app.Game;

		private event Action _close;

		public AddPlayerViewModel(BSMMApp app) {
			Title = "Add Player";
			_app = app;
		}
	}
}