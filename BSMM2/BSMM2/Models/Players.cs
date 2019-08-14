using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BSMM2.Models {

	public class Players {
		private const String DEFAULT_PREFIX = "Player";

		[JsonProperty]
		private String _prefix { get; set; }

		[JsonProperty]
		private List<Player> _players { get; set; }

		private IEnumerable<Player> Generate(int start, string prefix, int count = 1) {
			for (int i = 0; i < count; ++i) {
				yield return new Player(string.Format("{0}{1:000}", prefix, start + i));
			}
		}

		public Players(int count = 4, String prefix = DEFAULT_PREFIX) {
			_prefix = prefix;
			_players = Generate(1, prefix, count).ToList();
		}

		public void Add(int count = 1, String prefix = DEFAULT_PREFIX) {
			int start = _players.Count() + 1;
			_players.AddRange(Generate(start, prefix, count));
		}

		public void Add(String name)
			=> _players.Add(new Player(name));

		public void Remove(int index)
			=> _players.RemoveAt(index);

		[JsonIgnore]
		public IEnumerable<Player> Result
			=> _players.OrderByDescending(p => p);

		[JsonIgnore]
		public virtual IEnumerable<Player> Shuffle
			=> _players.OrderBy(i => Guid.NewGuid());//TODO : tentative

		public int Count => _players.Count;
	}
}