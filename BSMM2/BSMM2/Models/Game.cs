using BSMM2.Services;
using BSMM2.Modules.Rules.SingleMatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BSMM2.Modules.Rules;

namespace BSMM2.Models {

	[JsonObject(nameof(Game))]
	public class Game {

		private enum STATUS { Matching, Lock, Playing };

		[JsonProperty]
		private static string _prefix;

		[JsonProperty]
		private readonly Rule _rule;

		[JsonProperty]
		private readonly List<Player> _players;

		[JsonProperty]
		private readonly Stack<Round> _rounds;

		[JsonProperty]
		private Round _activeRound;

		[JsonProperty]
		private STATUS _status;

		[JsonProperty]
		private DateTime? _startTime;

		[JsonProperty]
		public bool AcceptByeMatchDuplication { get; set; } = false;

		[JsonProperty]
		public bool AcceptGapMatchDuplication { get; set; } = false;

		[JsonProperty]
		public virtual int TryCount { get; set; } = 10;

		[JsonIgnore]
		public TimeSpan? ElapsedTime
			=> DateTime.Now - _startTime;

		[JsonIgnore]
		public IEnumerable<Player> PlayerList
			=> _players.OrderByDescending(p => p);

		[JsonIgnore]
		public Round ActiveRound
			=> _activeRound ?? (_activeRound = Shuffle());

		[JsonIgnore]
		public IEnumerable<Round> Rounds
			=> _rounds;

		private string GenerateName(string name, int i) {
			return string.Format("{0}{1:000}", name, i);
		}

		private Game() {// For Serializer
		}

		public Game(Rule rule, int count, string prefix = "Player") : this(rule) {
			_prefix = prefix;
			for (int i = 0; i < count; ++i) {
				_players.Add(new Player(GenerateName(_prefix, i + 1)));
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
			_players = new List<Player>();
			_rounds = new Stack<Round>();
			_status = STATUS.Matching;
			_startTime = null;
		}

		public void Add(Player player) {
			_players.Add(player);
		}

		public void Add() {
			for (int i = _players.Count() + 1; ; ++i) {
				var name = GenerateName(_prefix, i);
				if (!_players.Any(player => player.Name == name)) {
					_players.Add(new Player(name));
					return;
				}
			}
		}

		public void Remove(Player player) {
			_players.Remove(player);
		}

		public bool CanExecuteShuffle()
			=> _status == STATUS.Matching;

		public Round Shuffle() {
			return _activeRound = MakeRound(_players, _rule);
		}

		public bool CanExecuteStepToLock()
			=> _status == STATUS.Matching;

		public void StepToLock() {
			_status = STATUS.Lock;
			ActiveRound.Lock();
		}

		public bool CanExecuteStepToPlaying()
			=> _status != STATUS.Playing;

		public void StepToPlaying() {
			StepToLock();
			ActiveRound.Commit();
			_status = STATUS.Playing;
			_startTime = DateTime.Now;
		}

		public bool CanExecuteBackToMatching()
			=> _status == STATUS.Lock;

		public void BackToMatching() {
			_activeRound.Unlock();
		}

		public bool CanExecuteStepToMatching()
			=> _status == STATUS.Playing && _activeRound.IsFinished;

		public void StepToMatching() {
			_status = STATUS.Matching;
			_startTime = null;
			_rounds.Push(ActiveRound);
			Shuffle();
		}

		protected virtual IEnumerable<Player> Shuffle(IEnumerable<Player> source) {
			return source.OrderBy(i => Guid.NewGuid());
		}

		private Round MakeRound(IEnumerable<Player> source, Rule rule) {
			for (int i = 0; i < TryCount; ++i) {
				var round = Create(Shuffle(source).Where(p => !p.Dropped).OrderByDescending(p => p, rule.CreateComparer()));
				if (round != null) {
					return round;
				}
			}
			return null;

			Round Create(IEnumerable<Player> players) {
				var results = new Queue<Match>();
				var stack = new List<Player>();

				foreach (var p1 in players) {
					if (!p1.Dropped) {
						var p2 = PickOpponent(stack, p1);
						if (p2 != null) {
							stack.Remove(p2);
							results.Enqueue(new Match(p2, p1));
						} else {
							stack.Add(p1);
						}
					}
				}
				switch (stack.Count) {
					case 0:
						return new Round(results);

					case 1: {
							var p = stack.First();
							if (AcceptByeMatchDuplication || !p.HasByeMatch) {
								results.Enqueue(new Match(p));
								return new Round(results);
							}
						}
						break;
				}
				return null;

				Player PickOpponent(IEnumerable<Player> opponents, Player player) {
					foreach (var opponent in opponents) {
						if (player.GetResult(opponent) == null) {
							if (AcceptGapMatchDuplication || !player.HasGapMatch) {
								return opponent;
							}
						}
					}
					return null;
				}
			}
		}
	}
}