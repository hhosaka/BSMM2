using BSMM2.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace BSMM2Test {

	public class FakeGame : Game {

		private class FakePlayers : Players {

			protected override IEnumerable<Player> Source(IEnumerable<Player> players)
				=> players;

			public FakePlayers(int count, String prefix)
				: base(count, prefix) {
			}

			public FakePlayers(TextReader r)
				: base(r) {
			}

			private FakePlayers() {// For Serializer
			}
		}

		public FakeGame(Rule rule, int count, string prefix = "Player")
			: base(rule, new FakePlayers(count, prefix)) {
		}

		public FakeGame(Rule rule, TextReader r)
			: base(rule, new FakePlayers(r)) {
		}
	}
}