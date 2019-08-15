using BSMM2.Services;
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
		private readonly Players _players;

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
		public Round ActiveRound
			=> _activeRound ?? (_activeRound = Shuffle());

		[JsonIgnore]
		public IEnumerable<Round> Rounds
			=> _rounds;

		protected virtual Players CreatePlayers(int count, String prefix)
			=> new Players(count, _prefix);

		private Game() {// For Serializer
		}

		public Game(Rule rule, int count, string prefix = "Player") : this(rule) {
			_prefix = prefix;
			_players = CreatePlayers(count, prefix);
		}

		public Game(Rule rule, TextReader reader) : this(rule) {
			string buf;
			while ((buf = reader.ReadLine()) != string.Empty) {
				_players.Add(buf);
			}
		}

		private Game(Rule rule) {
			_rule = rule;
			_rounds = new Stack<Round>();
			_status = STATUS.Matching;
			_startTime = null;
		}

		public void Add() {
			_players.Add();
		}

		public Players Players => _players;

		public bool CanExecuteShuffle()
			=> _status == STATUS.Matching;

		public Round Shuffle() {
			return _activeRound = MakeRound(_players.Shuffle, _rule);
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
				var round = Create(source.Where(p => !p.Dropped).OrderByDescending(p => p, rule.CreateComparer()));
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