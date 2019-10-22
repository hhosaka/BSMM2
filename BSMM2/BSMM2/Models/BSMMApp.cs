using BSMM2.Models.Matches;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BSMM2.Models {

	public class BSMMApp {
		private static BSMMApp _instance;

		private Dictionary<Guid, string> _games;
		private Engine _engine;

		public IDictionary<Guid, string> Games => _games;
		public IEnumerable<Rule> Rules { get; }

		public Rule Rule { get; set; }
		public Game Game { get; private set; }

		public static BSMMApp Instance => _instance;

		public BSMMApp() {
			Debug.Assert(_instance == null);

			Rules = new Rule[] {
				new SingleMatchRule(),
				new ThreeGameMatchRule(),
				new ThreeOnThreeMatchRule(),
			};
			Rule = Rules.First();
			_games = new Dictionary<Guid, string>();
			_engine = new Engine();
			_instance = this;
		}

		public bool Add(Game game, bool AsCurrentGame) {
			if (!_games.ContainsValue(game.Title)) {
				if (Game != null && AsCurrentGame && _games.ContainsKey(Game.Id)) {
					RemoveGame();
				}
				_games[game.Id] = game.Title;
				_engine.Save(game);
				Game = game;
				return true;
			}
			return false;
		}

		public void RemoveGame() {
			_engine.Remove(Game);
			_games.Remove(Game.Id);
			Game = null;
		}

		public Game Select(Guid id) {
			return Game = _engine.Load(id);
		}

		public void Save() {
			_engine.Save(Game);
		}
	}
}