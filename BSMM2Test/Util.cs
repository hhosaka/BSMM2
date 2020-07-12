﻿using BSMM2.Models;
using BSMM2.Models.Matches.SingleMatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BSMM2Test {

	internal class Util {
		public static readonly string DefaultOrigin = "Player";

		public static int ConvId(string origin, string name) {
			if (!name.StartsWith(origin)) {
				return -1;
			} else {
				Assert.AreEqual(0, name.IndexOf(origin));
				return int.Parse(name.Substring(name.Length - 3));
			}
		}

		private static void Check(IEnumerable<int> expect, IEnumerable<Player> players) {
			Check(expect, DefaultOrigin, players);
		}

		public static void CheckWithOrder(IEnumerable<int> expectedPlayers, IEnumerable<int> expectedOrder, IEnumerable<Player> players) {
			Check(expectedPlayers, players);
			var result = players.Select(p => p.Order);
			CollectionAssert.AreEqual(expectedOrder.ToArray(), result.ToArray(), Message(expectedOrder, result));
		}

		public static void CheckOrder(IEnumerable<int> expectedOrder, IEnumerable<Player> players) {
			var result = players.Select(p => p.Order);
			CollectionAssert.AreEqual(expectedOrder.ToArray(), result.ToArray(), Message(expectedOrder, result));
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

		public static void Check(IResult a, IResult b) {
			if (a == null && b == null) return;//どちらも未設定
			Assert.IsNotNull(a);
			Assert.IsNotNull(b);
			Assert.AreEqual(a.IsFinished, b.IsFinished);
			Assert.AreEqual(a.MatchPoint, b.MatchPoint);
			Assert.AreEqual(a.WinPoint, b.WinPoint);
			Assert.AreEqual(a.LifePoint, b.LifePoint);
		}

		public static void Check(IPlayer a, IPlayer b) {
			Assert.AreEqual(a.Dropped, b.Dropped);
			Assert.AreEqual(a.Name, b.Name);
			Assert.AreEqual(a.HasByeMatch, b.HasByeMatch);
			Assert.AreEqual(a.HasGapMatch, b.HasGapMatch);
			Assert.AreEqual(0, a.Point.Value, b.Point.Value);
		}

		public static void Check(IRule rule, Players a, Players b) {
			Assert.AreEqual(a.Count, b.Count);
			var ita = a.GetByOrder().GetEnumerator();
			var itb = b.GetByOrder().GetEnumerator();
			while (ita.MoveNext() && itb.MoveNext()) {
				Check(ita.Current, itb.Current);
			}
		}

		public static void Check(Match a, Match b) {
			Check(a.Record1.Player, b.Record1.Player);
			Check(a.Record2.Player, b.Record2.Player);
			Check(a.Record1.Result, b.Record1.Result);
			Check(a.Record2.Result, b.Record2.Result);
		}

		public static void Check(Round a, Round b) {
			Assert.AreEqual(a.IsFinished, b.IsFinished);
			Assert.AreEqual(a.IsPlaying, b.IsPlaying);
			Check(a.Matches, b.Matches);
		}

		public static void Check(IEnumerable<Match> a, IEnumerable<Match> b) {
			var ita = a.GetEnumerator();
			var itb = b.GetEnumerator();
			while (ita.MoveNext() && itb.MoveNext()) {
				Check(ita.Current as Match, itb.Current as Match);
			}
		}

		public static void Check(IEnumerable<Round> a, IEnumerable<Round> b) {
			var ita = a.GetEnumerator();
			var itb = b.GetEnumerator();
			while (ita.MoveNext() && itb.MoveNext()) {
				Check(ita.Current as Round, itb.Current as Round);
			}
		}

		public static void Check(Game a, Game b) {
			Assert.AreEqual(a.Title, b.Title);
			Assert.AreEqual(a.Id, b.Id);
			Assert.AreEqual(a.StartTime, b.StartTime);
			Assert.AreEqual(a.Rule.Name, b.Rule.Name);
			Assert.AreEqual(a.AcceptByeMatchDuplication, b.AcceptByeMatchDuplication);
			Assert.AreEqual(a.AcceptGapMatchDuplication, b.AcceptGapMatchDuplication);
			if (a.Rule is SingleMatchRule srule) {
				Assert.AreEqual(srule.EnableLifePoint, srule.EnableLifePoint);
			}
			Check(a.Rule, a.Players, b.Players);
			Check(a.ActiveRound, b.ActiveRound);
			Check(a.Rounds, b.Rounds);
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

		public static Match GetMatch(Game game, int index)
			=> game.ActiveRound.Matches.ElementAt(index);

		public static void SetResult(Game game, int index, RESULT_T result)
			=> GetMatch(game, index).SetResult(result);

		public static string Export(Game game) {
			var buf = new StringBuilder();
			game.Players.Export(new StringWriter(buf));
			return buf.ToString();
		}
	}
}