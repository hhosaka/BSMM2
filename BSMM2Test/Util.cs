﻿using BSMM2.Models;
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
			var result = players.Select(player => ConvId(origin, player.Name));
			CollectionAssert.AreEqual(expect.ToArray(), result.ToArray(), Message(expect, result));
		}

		public static void Check(IEnumerable<int> expect, Round round) {
			Check(expect, DefaultOrigin, round);
		}

		public static void Check(IEnumerable<int> expect, string origin, Round round) {
			var result = round.Matches.SelectMany(match => match.PlayerNames.Select(name => ConvId(origin, name)));
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