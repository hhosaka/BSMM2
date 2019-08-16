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

		[JsonProperty]
		public Rule _rule;

		[JsonProperty]
		private readonly Players _players;

		[JsonProperty]
		private readonly Stack<Round> _rounds;

		[JsonProperty]
		private IRound _activeRound;

		[JsonProperty]
		private DateTime? _startTime;

		[JsonProperty]
		public bool AcceptByeMatchDuplication { get; set; } = false;

		[JsonProperty]
		public bool AcceptGapMatchDuplication { get; set; } = false;

		[JsonProperty]
		public virtual int TryCount { get; set; } = 100;

		[JsonIgnore]
		public TimeSpan? ElapsedTime
			=> DateTime.Now - _startTime;

		[JsonIgnore]
		public Players Players => _players;

		[JsonIgnore]
		public IEnumerable<Round> Rounds
			=> _rounds;

		[JsonIgnore]
		public IRound ActiveRound => _activeRound;

		[JsonIgnore]
		public bool Locked => (_activeRound as Matching)?.Locked == true;

		public Game() {// For Serializer
		}

		public Game(Rule rule, Players players) {
			_players = players;
			_rule = rule;
			_rounds = new Stack<Round>();
			_startTime = null;
			Shuffle(true);
		}

		[JsonIgnore]
		public bool CanExecuteShuffle
			=> _activeRound is Matching;

		public void Shuffle(bool force = false) {
			if (force || (_activeRound as Matching)?.Locked == false)
				_activeRound = new Matching(MakeRound(_players.Shuffle, _rule));
		}

		public bool CanExecuteStepToLock()
			=> (_activeRound as Matching).Locked == false;

		public void StepToLock() {
			(_activeRound as Matching)?.Lock();
		}

		public bool CanExecuteStepToPlaying()
			=> _activeRound is Matching;

		public void StepToPlaying() {
			_activeRound = new Round(_activeRound.Matches);
			_startTime = DateTime.Now;
		}

		public bool CanExecuteBackToMatching()
			=> (_activeRound as Matching).Locked == true;

		public void BackToMatching()
			=> (_activeRound as Matching)?.Unlock();

		public bool CanExecuteStepToMatching()
			=> (_activeRound as Round)?.IsFinished == true;

		public void StepToMatching() {
			_startTime = null;
			_rounds.Push((Round)_activeRound);
			Shuffle(true);
		}

		private IEnumerable<Match> MakeRound(IEnumerable<Player> source, Rule rule) {
			for (int i = 0; i < TryCount; ++i) {
				var matchingList = Create(source.Where(p => !p.Dropped).OrderByDescending(p => p, rule.CreateComparer()));
				if (matchingList != null) {
					return matchingList;
				}
			}
			return null;

			IEnumerable<Match> Create(IEnumerable<Player> players) {
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
						return results;

					case 1: {
							var p = stack.First();
							if (AcceptByeMatchDuplication || !p.HasByeMatch) {
								results.Enqueue(new Match(p));
								return results;
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