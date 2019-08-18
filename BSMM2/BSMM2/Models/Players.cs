using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSMM2.Models {

	public class Players {
		private const String DEFAULT_PREFIX = "Player";

		[JsonProperty]
		private String _prefix;

		[JsonProperty]
		private List<Player> _players;

		private IEnumerable<Player> Generate(int start, string prefix, int count = 1) {
			for (int i = 0; i < count; ++i) {
				yield return new Player(string.Format("{0}{1:000}", prefix, start + i));
			}
		}

		private IEnumerable<Player> Generate(TextReader r) {
			String buf;
			while ((buf = r.ReadLine()) != null) {
				if (!String.IsNullOrWhiteSpace(buf))
					yield return new Player(buf);
			}
		}

		public Players() {
		}

		public Players(int count, String prefix = DEFAULT_PREFIX) {
			_prefix = prefix;
			_players = Generate(1, prefix, count).ToList();
		}

		public Players(TextReader r, String prefix = DEFAULT_PREFIX) {
			_prefix = prefix;
			_players = Generate(r).ToList();
		}

		public void Add(int count = 1, String prefix = DEFAULT_PREFIX) {
			int start = _players.Count() + 1;
			_players.AddRange(Generate(start, prefix, count));
		}

		public void Add(String name)
			=> _players.Add(new Player(name));

		public void Remove(int index)
			=> _players.RemoveAt(index);

		public void Remove(Player player)
			=> _players.Remove(player);

		public IEnumerable<Player> GetPlayersByOrder(Rule rule) {
			Reset(rule);
			return GetPlayers(rule, _players, 0, true);
		}

		public IEnumerable<Player> GetSource(Rule rule, int level) {
			return GetPlayers(rule, Source(_players), level, false);
		}

		private IEnumerable<Player> GetPlayers(Rule rule, IEnumerable<Player> players, int level, bool strictly) {
			return players.OrderByDescending(p => p, rule.CreateComparer());
		}

		public void Reset(Rule rule) {
			_players.ForEach(p => p.CalcResult(rule));
			_players.ForEach(p => p.CalcOpponentResult(rule));
		}

		protected virtual IEnumerable<Player> Source(IEnumerable<Player> players)
			=> players.OrderBy(i => Guid.NewGuid());

		[JsonIgnore]
		public int Count => _players.Count;
	}
}