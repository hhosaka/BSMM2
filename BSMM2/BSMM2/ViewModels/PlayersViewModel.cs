﻿using BSMM2.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

		public DelegateCommand NewGameCommand { get; }
		public DelegateCommand RuleCommand { get; }
		public DelegateCommand SelectGameCommand { get; }
		public DelegateCommand DeleteGameCommand { get; }
		public DelegateCommand AddPlayerCommand { get; }
		public DelegateCommand ExportPlayersCommand { get; }
		public DelegateCommand HelpCommand { get; }

		public PlayersViewModel(BSMMApp app, Action newGame = null, Action openRule = null, Action selectGame = null, Action deleteGame = null, Action addPlayer = null) {
			_app = app;
			Players = new ObservableCollection<Player>();

			NewGameCommand = new DelegateCommand(() => newGame?.Invoke());
			RuleCommand = new DelegateCommand(() => openRule?.Invoke(), () => _app.Game.CanAddPlayers());
			SelectGameCommand = new DelegateCommand(() => selectGame?.Invoke(), () => _app.Games.Any());
			DeleteGameCommand = new DelegateCommand(() => deleteGame?.Invoke(), () => _app.Games.Any());
			AddPlayerCommand = new DelegateCommand(() => addPlayer?.Invoke(), () => _app.Game.CanAddPlayers());
			ExportPlayersCommand = new DelegateCommand(_app.ExportPlayers);

			MessagingCenter.Subscribe<object>(this, Messages.REFRESH,
				async (sender) => await ExecuteRefresh());

			Refresh();
		}

		public async Task ExecuteRefresh() {
			if (!IsBusy) {
				IsBusy = true;

				try {
					await Task.Run(() => Refresh());
				} finally {
					IsBusy = false;
				}
			}
		}

		private void Refresh() {
			Players = new ObservableCollection<Player>(Game.Players.GetByOrder());
			Title = Game.Headline;
			RuleCommand?.RaiseCanExecuteChanged();
			SelectGameCommand?.RaiseCanExecuteChanged();
			DeleteGameCommand?.RaiseCanExecuteChanged();
			AddPlayerCommand?.RaiseCanExecuteChanged();
		}
	}
}