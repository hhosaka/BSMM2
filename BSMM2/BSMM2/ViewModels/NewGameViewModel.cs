﻿using BSMM2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	internal class PlayerControlConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string v && parameter is string p)
				return v == p;

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}

	internal class NewGameViewModel : BaseViewModel {
		public string GameName { get; set; }
		public Rule Rule { get; set; }
		public IEnumerable<Rule> Rules => BSMMApp.Instance.Rules;
		public string Prefix { get; set; }
		public int PlayerCount { get; set; }
		public string EntrySheet { get; set; }
		public bool EnableLifePoint { get; set; }
		public bool AsCurrentGame { get; set; }

		private string _PlayerMode;

		public string PlayerMode {
			get => _PlayerMode;
			set => SetProperty<string>(ref _PlayerMode, value);
		}

		public void ExecuteNewGame() {
			var game = new Game(Rule, CreatePlayers(), EnableLifePoint, GameName);
			BSMMApp.Instance.Add(game, AsCurrentGame);
			MessagingCenter.Send(this, "NewGame", game);
		}

		private Players CreatePlayers() {
			switch (PlayerMode) {
				case "Number":
					return new Players(PlayerCount, Prefix);

				case "EntrySheet": {
						using (var reader = new StringReader(EntrySheet)) {
							return new Players(reader);
						}
					}
				default:
					throw new ArgumentException();
			}
		}

		public NewGameViewModel() {
			Title = "Create New Game";
			Rule = Rules.First();
			GameName = "Game" + DateTime.Now.ToShortDateString();
			PlayerMode = "Number";
			Prefix = "Player";
			PlayerCount = 8;
			EntrySheet = "Player 1\nPlayer 2\nPlayer 3\n";
			AsCurrentGame = true;
		}
	}
}