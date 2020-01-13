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
			public Guid Id { get; }
			public string Title { get; }

			public ICommand RemoveCommand { get; }

			public Gameset(GamesViewModel parent, Guid id, string title) {
				_parent = parent;
				Id = id;
				Title = title;
				RemoveCommand = new Command(() => parent.Remove(Id));
			}
		}

		private BSMMApp _app;
		private IGame Game => _app.Game;

		private event Action _close;

		private IEnumerable<Gameset> _games;

		public IEnumerable<Gameset> Games {
			get => _games;
			set { SetProperty(ref _games, value); }
		}

		public GamesViewModel(BSMMApp app, Action close) {
			_app = app;
			_close += close;
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
			Games = _app.Games.Select(game => new Gameset(this, game.Key, game.Value));
			Title = Game.Headline;
		}

		public void Select(Guid id) {
			if (_app.Select(id) != null) {
				MessagingCenter.Send<object>(this, "UpdatedRound");
			}
		}

		public async void Remove(Guid id) {
			if (_app.Remove(id)) {
				MessagingCenter.Send<object>(this, "UpdatedRound");
				await Refresh();
			}
		}
	}
}