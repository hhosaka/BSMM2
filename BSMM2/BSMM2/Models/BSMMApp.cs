using BSMM2.Models.Matches;
using BSMM2.Models.Matches.SingleMatch;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	public class BSMMApp {
		private Dictionary<Guid, string> _games;
		private Engine _engine;

		public IDictionary<Guid, string> Games => _games;
		public IEnumerable<Rule> Rules { get; }

		public Rule Rule { get; set; }
		public Game Game { get; private set; }

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

		public ContentPage CreateMatchPage(Match match) {
			return Game.CreateMatchPage(match);
		}
	}
}