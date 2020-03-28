using BSMM2.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class GamesViewModel : BaseViewModel {
		private BSMMApp _app;
		private IGame Game => _app.Game;

		public IEnumerable<IGame> Games => _app.Games;

		private Action<IGame> _action;

		private IGame _selectedItem;

		public IGame SelectedItem {
			get => _selectedItem;
			set {
				SetProperty(ref _selectedItem, value);
				if (_selectedItem != null) {
					_action?.Invoke(_selectedItem);
				}
			}
		}

		public GamesViewModel(BSMMApp app, string title, Action<IGame> action) {
			_app = app;
			SelectedItem = app.Game;
			_action = action;
			Title = title;
		}

		public void Select(IGame game) {
			if (_app.Select(game)) {
				MessagingCenter.Send<object>(this, Messages.REFRESH);
			}
		}

		public void Remove(IGame game) {
			if (_app.Remove(game)) {
				MessagingCenter.Send<object>(this, Messages.REFRESH);
			}
		}
	}
}