using BSMM2.Models.Matches;
using BSMM2.Models.Matches.SingleMatch;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	public class BSMMApp {
		private static Game defaultGame = new Game();

		private Dictionary<Guid, string> _games;
		private Game _game;
		private Engine _engine;

		public IDictionary<Guid, string> Games => _games;
		public IEnumerable<Rule> Rules { get; }

		public Rule Rule { get; set; }
		public Game Game => _game ?? defaultGame;

		public BSMMApp() {
			Rules = new Rule[] {
				new SingleMatchRule(),
				new ThreeGameMatchRule(),
				new ThreeOnThreeMatchRule(),
			};
			Rule = Rules.First();
			_games = new Dictionary<Guid, string>();
			_engine = new Engine();
		}

		public bool Add(Game game, bool AsCurrentGame) {
			if (!_games.ContainsValue(game.Title)) {
				if (_game != null && AsCurrentGame && _games.ContainsKey(_game.Id)) {
					RemoveGame();
				}
				_games[game.Id] = game.Title;
				_engine.Save(game);
				_game = game;
				return true;
			}
			return false;
		}

		public void RemoveGame() {
			if (_game != null) {
				_engine.Remove(Game);
				_games.Remove(Game.Id);
				_game = null;
			}
		}

		public Game Select(Guid id) {
			return _game = _engine.Load(id);
		}

		public void Save() {
			if (_game != null) {
				_engine.Save(_game);
			}
		}

		public ContentPage CreateMatchPage(Match match) {
			return _game.CreateMatchPage(match);
		}

		public DelegateCommand CreateStartCommand(Action onChanged)
			=> new DelegateCommand(() => { _game?.StepToPlaying(); onChanged(); }, () => _game?.CanExecuteStepToPlaying() == true);
	}
}