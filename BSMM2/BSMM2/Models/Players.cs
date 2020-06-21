﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BSMM2.Models {

	public class Players {
		private const String DEFAULT_PREFIX = "Player";

		[JsonProperty]
		private Rule _rule;

		[JsonProperty]
		private String _prefix;

		[JsonProperty]
		private List<Player> _players;

		[JsonIgnore]
		public int Count => _players.Count;

		private IEnumerable<Player> Generate(int start, string prefix, int count = 1) {
			Debug.Assert(count > 0);
			for (int i = 0; i < count; ++i) {
				yield return new Player(_rule, string.Format("{0}{1:000}", prefix, start + i));
			}
		}

		private IEnumerable<Player> Generate(TextReader r) {
			String buf;
			while ((buf = r.ReadLine()) != null) {
				if (!String.IsNullOrWhiteSpace(buf))
					yield return new Player(_rule, buf);
			}
		}

		public Players() {
		}

		public Players(Rule rule, int count, String prefix = DEFAULT_PREFIX) {
			_rule = rule;
			_prefix = prefix;
			_players = Generate(1, prefix, count).ToList();
		}

		public Players(Rule rule, TextReader r, String prefix = DEFAULT_PREFIX) {
			_rule = rule;
			_prefix = prefix;
			_players = Generate(r).ToList();
		}

		public void Add(int count = 1, String prefix = DEFAULT_PREFIX) {
			int start = _players.Count() + 1;
			_players.AddRange(Generate(start, prefix, count));
		}

		public void Add(String name)
			=> _players.Add(new Player(_rule, name));

		public void Remove(int index)
			=> _players.RemoveAt(index);

		public void Remove(Player player)
			=> _players.Remove(player);

		public IEnumerable<Player> GetByOrder() {
			if (_players == null) {
				return Enumerable.Empty<Player>();
			} else {
				Reset();
				var comparer = _rule.GetComparer(true);
				var players = _players.OrderByDescending(p => p, comparer);
				Player prev = null;
				int order = 0;
				int count = 0;
				foreach (var p in players) {
					if (prev == null || comparer.Compare(prev, p) != 0) {
						order = count;
						prev = p;
					}
					p.Order = order + 1;
					++count;
				}
				return players;
			}
		}

		public IEnumerable<Player> GetSource(Rule rule)
			=> Source(_players).OrderByDescending(p => p, rule.GetComparer(false));

		public void Reset() {
			_players.ForEach(p => p.CalcPoint(_rule));
			_players.ForEach(p => p.CalcOpponentPoint(_rule));
		}

		protected virtual IEnumerable<Player> Source(IEnumerable<Player> players)
			=> players.OrderBy(i => Guid.NewGuid());

		public void Export(TextWriter writer) {
			_players.First()?.ExportTitle(writer);
			writer.WriteLine();
			foreach (var player in GetByOrder()) {
				player.ExportData(writer);
				writer.WriteLine();
			}
		}
	}
}