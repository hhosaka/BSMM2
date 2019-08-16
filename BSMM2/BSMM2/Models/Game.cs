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
		public Rule _rule;

		[JsonProperty]
		private readonly Players _players;

		[JsonProperty]
		private readonly Stack<Round> _rounds;

		[JsonProperty]
		private MatchingList _matchingList;

		[JsonProperty]
		private STATUS _status;

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
		public MatchingList MatchingList
			=> _matchingList ?? (_matchingList = Shuffle());

		[JsonIgnore]
		public Players Players => _players;

		[JsonIgnore]
		public IEnumerable<Round> Rounds
			=> _rounds;

		[JsonIgnore]
		public Round ActiveRound => _rounds.Peek();

		private Game() {// For Serializer
		}

		public Game(Rule rule, Players players) {
			_players = players;
			_rule = rule;
			_rounds = new Stack<Round>();
			_status = STATUS.Matching;
			_startTime = null;
		}

		[JsonIgnore]
		public bool CanExecuteShuffle
			=> _status == STATUS.Matching;

		public MatchingList Shuffle() {
			return _matchingList = new MatchingList(MakeRound(_players.Shuffle, _rule));
		}

		public bool CanExecuteStepToLock()
			=> _status == STATUS.Matching;

		public void StepToLock() {
			_status = STATUS.Lock;
			MatchingList.Locked = true;
		}

		public bool CanExecuteStepToPlaying()
			=> _status != STATUS.Playing;

		public void StepToPlaying() {
			StepToLock();
			var round = new Round(_matchingList.Matches);
			round.Commit();
			_rounds.Push(round);
			_matchingList = null;
			_status = STATUS.Playing;
			_startTime = DateTime.Now;
		}

		public bool CanExecuteBackToMatching()
			=> _status == STATUS.Lock;

		public void BackToMatching() {
			_matchingList.Locked = false;
		}

		public bool CanExecuteStepToMatching()
			=> _status == STATUS.Playing && ActiveRound.IsFinished;

		public void StepToMatching() {
			_status = STATUS.Matching;
			_startTime = null;
			Shuffle();
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