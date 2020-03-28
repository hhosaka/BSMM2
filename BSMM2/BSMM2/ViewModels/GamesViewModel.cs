using BSMM2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class GamesViewModel : BaseViewModel {

		public class Gameset {
			private GamesViewModel _parent;
			public IGame Game { get; }

			public ICommand RemoveCommand { get; }

			public Gameset(GamesViewModel parent, IGame game) {
				_parent = parent;
				Game = game;
				RemoveCommand = new Command(() => parent.Remove(Game));
			}
		}

		private BSMMApp _app;
		private IGame Game => _app.Game;

		private IEnumerable<Gameset> _games;

		public IEnumerable<Gameset> Games {
			get => _games;
			set => SetProperty(ref _games, value);
		}

		public GamesViewModel(BSMMApp app, Action close) {
			_app = app;
			UpdateList();
		}

		public async Task Refresh() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => UpdateList());
				} finally {
					IsBusy = false;
				}
			}
		}

		public void UpdateList() {
			Games = _app.Games.Select(game => new Gameset(this, game));
			Title = Game.Headline;
		}

		public void Select(IGame game) {
			if (_app.Select(game)) {
				MessagingCenter.Send<object>(this, Messages.REFRESH);
			}
		}

		public async void Remove(IGame game) {
			if (_app.Remove(game)) {
				MessagingCenter.Send<object>(this, Messages.REFRESH);
				await Refresh();
			}
		}
	}
}