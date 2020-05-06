using BSMM2.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace BSMM2Test {

	public class FakeGame : Game {

		private class FakePlayers : Players {

			protected override IEnumerable<Player> Source(IEnumerable<Player> players)
				=> players;

			public FakePlayers(Rule rule, int count, String prefix)
				: base(rule, count, prefix) {
			}

			public FakePlayers(Rule rule, TextReader r)
				: base(rule, r) {
			}

			private FakePlayers() {// For Serializer
			}
		}

		public FakeGame() {
		}

		public FakeGame(Rule rule, int count, string prefix = "Player")
			: base(rule, new FakePlayers(rule, count, prefix), DateTime.Now.ToString()) {
		}

		public FakeGame(Rule rule, TextReader r)
			: base(rule, new FakePlayers(rule, r), DateTime.Now.ToString()) {
		}
	}
}