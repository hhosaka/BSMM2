using BSMM2.Models;
using BSMM2.Modules.Rules.SingleMatch;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMM2Test {

	public class FakeGame : Game {

		private class FakePlayers : Players {

			public override IEnumerable<Player> Shuffle
				=> Source;

			public FakePlayers(int count, String prefix)
				: base(count, prefix) {
			}
		}

		protected override Players CreatePlayers(int count, String prefix) {
			return new FakePlayers(count, prefix);
		}

		protected override IEnumerable<Player> Shuffle(IEnumerable<Player> source) {
			return source;
		}

		public FakeGame(Rule rule, int count, string prefix = "Player")
			: base(rule, count, prefix) {
		}
	}
}