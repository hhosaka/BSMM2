using BSMM2.Models;
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

		public static void CheckOrder(IEnumerable<int> expect, IEnumerable<Player> players) {
			var result = players.Select(p => p.Order);
			CollectionAssert.AreEqual(expect.ToArray(), result.ToArray(), Message(expect, result));
		}

		public static void Check(IEnumerable<int> expect, string origin, IEnumerable<Player> players) {
			var result = players.Select(player => ConvId(origin, player.Name));
			CollectionAssert.AreEqual(expect.ToArray(), result.ToArray(), Message(expect, result));
		}

		public static void Check(IEnumerable<int> expect, Matching matchingList) {
			Check(expect, DefaultOrigin, matchingList.Matches);
		}

		public static void Check(IEnumerable<int> expect, IRound round) {
			Check(expect, DefaultOrigin, round.Matches);
		}

		public static void Check(IEnumerable<int> expect, string origin, Match[] matches) {
			var result = matches.SelectMany(match => match.PlayerNames.Select(name => ConvId(origin, name)));
			CollectionAssert.AreEqual(expect.ToArray(), result.ToArray(), Message(expect, result));
		}

		private static String Message(IEnumerable<int> expect, IEnumerable<int> result) {
			return "result=" + ToString(result) + " expected=" + ToString(expect);

			String ToString(IEnumerable<int> array) {
				var builder = new StringBuilder();
				builder.Append("{");
				array.ToList().ForEach(i => builder.Append(i.ToString() + ","));
				builder.Append("}");
				return builder.ToString();
			}
		}
	}
}