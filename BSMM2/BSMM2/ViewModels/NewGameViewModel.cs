using BSMM2.Models;
using BSMM2.Resource;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
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
		private BSMMApp _app;
		public string GameName { get; set; }
		public Rule Rule { get; set; }
		public IEnumerable<Rule> Rules => _app.Rules;
		public string Prefix { get; set; }
		public int PlayerCount { get; set; }
		public string EntrySheet { get; set; }
		public bool EnableLifePoint { get; set; }
		public bool AsCurrentGame { get; set; }

		public string TextAsCurrentGame => AppResources.TextAsCurrentGame;
		public string TextEnableLifePoint => AppResources.TextEnableLifePoint;
		private string _playerMode;

		public string PlayerMode {
			get => _playerMode;
			set => SetProperty<string>(ref _playerMode, value);
		}

		public ICommand CreateCommand { get; }

		private event Action _closeWindow;

		private Players CreatePlayers(Rule rule) {
			switch (PlayerMode) {
				case "Number":
					return new Players(rule, PlayerCount, Prefix);

				case "EntrySheet": {
						using (var reader = new StringReader(EntrySheet)) {
							return new Players(rule, reader);
						}
					}
				default:
					throw new ArgumentException();
			}
		}

		public NewGameViewModel(BSMMApp app, Action closeWindow) {
			_app = app;
			Title = "Create New Game";
			Rule = Rules.First();
			GameName = Game.GenerateTitle();
			PlayerMode = "Number";
			Prefix = "Player";
			PlayerCount = 8;
			EntrySheet = "Player 1\nPlayer 2\nPlayer 3\n";
			AsCurrentGame = true;
			_closeWindow += closeWindow;
			CreateCommand = new DelegateCommand(ExecuteCreate);

			void ExecuteCreate() {
				if (app.Add(new Game(Rule.Clone(), CreatePlayers(Rule), GameName), AsCurrentGame)) {
					MessagingCenter.Send<object>(this, Messages.REFRESH);
				}// TODO : Error handling is required?
				_closeWindow?.Invoke();
			}
		}
	}
}