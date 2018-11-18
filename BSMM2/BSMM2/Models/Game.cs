using BSMM2.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BSMM2.Models {

	public class Game {
		private string _title = "Player";
		private readonly Rule _rule;
		private readonly List<Player> _players = new List<Player>();
		private readonly Stack<Round> _rounds = new Stack<Round>();
		private DateTime _startTime;

		public Game(Rule rule, int count) : this(rule) {
			for (int i = 0; i < count; ++i) {
				_players.Add(new Player(string.Format("{0}{1:000}", _title, i + 1)));
			}
		}

		public Game(Rule rule, TextReader reader) : this(rule) {
			string buf;
			while ((buf = reader.ReadLine()) != string.Empty) {
				_players.Add(new Player(buf));
			}
		}

		private Game(Rule rule) {
			_rule = rule;
		}

		public void Add(Player player) {
			var index = _players.IndexOf(_players.Find(p => p.Name.CompareTo(player.Name) > 0));
			if (index == -1) {
				_players.Add(player);
			} else {
				_players.Insert(index, player);
			}
		}

		public void Delete(Player player) {
			_players.Remove(player);
		}

		public IEnumerable<Player> PlayerList
			=> _players.OrderByDescending(p => p, _rule.CreateComparer());

		public Round ActiveRound { get; private set; }

		private IEnumerable<Player> Shuffle(IEnumerable<Player> source) {
#if DEBUG
			return source;
#else
			return source.OrderBy(i => Guid.NewGuid());
#endif
		}

		public bool Shuffle() =>
			(ActiveRound = _rule.MakeRound(() => Shuffle(_players))) != null;

		public void Start() {
			_startTime = DateTime.Now;
		}

		public TimeSpan ElapsedTime
			=> _startTime - DateTime.Now;

		public bool Matching() {
			_rounds.Push(ActiveRound);
			_startTime = DateTime.MinValue;
			return Shuffle();
		}

		public IEnumerable<Round> Rounds => _rounds;

		protected virtual int TryCount => 10;
	}
}