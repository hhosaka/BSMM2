using BSMM2.Models;
using BSMM2.Modules.Rules.SingleMatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSMM2Test {

	internal class Util {
		public static readonly string DefaultOrigin = "Player";

		public static int ConvId(string origin, string name) {
			if (name == "BYE") {
				return -1;
			} else {
				Assert.AreEqual(0, name.IndexOf(origin));
				return int.Parse(name.Substring(name.Length - 3));
			}
		}

		public static void Check(IEnumerable<int> expect, IEnumerable<Player> players) {
			Check(expect, DefaultOrigin, players);
		}

		public static void Check(IEnumerable<int> expect, string origin, IEnumerable<Player> players) {
			CollectionAssert.AreEqual(expect.ToArray(), players.Select(player => ConvId(origin, player.Name)).ToArray());
		}

		public static void Check(IEnumerable<int> expect, Round round) {
			Check(expect, DefaultOrigin, round);
		}

		public static void Check(IEnumerable<int> expect, string origin, Round round) {
			var buf = round.Matches.SelectMany(match => match.PlayerNames.Select(name => ConvId(origin, name))).ToArray();
			CollectionAssert.AreEqual(expect.ToArray(),
				round.Matches.SelectMany(match => match.PlayerNames.Select(name => ConvId(origin, name))).ToArray());
		}
	}
}