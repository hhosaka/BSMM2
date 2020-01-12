using BSMM2.Models.Matches;
using BSMM2.Models.Matches.SingleMatch;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	public class BSMMApp {
		private static readonly IGame defaultGame = new DefaultGame();

		private Dictionary<Guid, string> _games;
		private Game _game;
		private Engine _engine;

		public IDictionary<Guid, string> Games => _games;
		public IEnumerable<Rule> Rules { get; }

		public Rule Rule { get; set; }
		public IGame Game => _game ?? defaultGame;

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
			if (_game != null && AsCurrentGame && _games.ContainsKey(_game.Id)) {
				Remove(_game.Id);
			}
			_games[game.Id] = game.Title;
			_engine.Save(game);
			_game = game;
			return true;
		}

		public bool Remove(Guid id) {
			Debug.Assert(id != Guid.Empty);
			_engine.Remove(_game);
			_games.Remove(_game.Id);
			_game = null;
			return true;
		}

		public Game Select(Guid id) {
			var game = _engine.Load(id);
			if (game != null) {
				_game = game;
			}
			return game;
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
			=> new DelegateCommand(() => { Game.StepToPlaying(); onChanged(); }, () => Game.CanExecuteStepToPlaying);
	}
}