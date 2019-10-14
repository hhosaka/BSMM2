using BSMM2.Models.Matches;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BSMM2.Models {

	public class BSMMApp {
		public IEnumerable<Rule> Rules { get; }
		public Rule Rule { get; set; }
		public HashSet<Game> Games { get; }
		public Game Game { get; private set; }

		private static BSMMApp _instance;

		public static BSMMApp Instance => _instance;

		public BSMMApp() {
			Debug.Assert(_instance == null);

			Rules = new Rule[] {
				new SingleMatchRule(),
				new ThreeGameMatchRule(),
				new ThreeOnThreeMatchRule(),
			};
			Rule = Rules.First();
			Games = new HashSet<Game>();
			_instance = this;
		}

		public Game Add(Game game, bool AsCurrentGame) {
			if (AsCurrentGame && Games.Contains(Game)) {
				Games.Remove(Game);
			}
			Games.Add(game);
			return Game = game;
		}
	}
}