using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject]
	public class Game {

		[JsonProperty]
		public string Title { get; set; }

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

		public Game(Rule rule, Players players, string title = "Some Game") {
			Title = title;
			_players = players;
			_rule = rule;
			_rounds = new Stack<Round>();
			_startTime = null;
			Shuffle(true);
		}

		public bool CanExecuteShuffle()
			=> (_activeRound as Matching)?.Locked == false;

		public void Shuffle(bool force = false) {
			if (force || CanExecuteShuffle())
				_activeRound = new Matching(MakeRound());
		}

		public bool CanExecuteStepToLock()
			=> (_activeRound as Matching).Locked == false;

		public void StepToLock() {
			if (CanExecuteStepToLock()) {
				(_activeRound as Matching)?.Lock();
			}
		}

		public bool CanExecuteStepToPlaying()
			=> _activeRound is Matching;

		public void StepToPlaying() {
			if (CanExecuteStepToPlaying()) {
				_activeRound = new Round(_activeRound.Matches);
				_startTime = DateTime.Now;
			}
		}

		public bool CanExecuteBackToMatching()
			=> (_activeRound as Matching).Locked == true;

		public void BackToMatching() {
			if (CanExecuteBackToMatching()) {
				(_activeRound as Matching)?.Unlock();
			}
		}

		public bool CanExecuteStepToMatching()
			=> (_activeRound as Round)?.IsFinished == true;

		public void StepToMatching() {
			if (CanExecuteStepToMatching()) {
				_startTime = null;
				_rounds.Push((Round)_activeRound);
				Shuffle(true);
			}
		}

		private IEnumerable<Player> GetOrderedPlayers() {
			var comparer = _rule.CreateOrderComparer();
			var players = _players.GetByOrder(_rule);
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

		public IEnumerable<Player> PlayersByOrder
				=> GetOrderedPlayers();

		private IEnumerable<Match> MakeRound() {
			_players.Reset(_rule);
			for (int level = 0; level < _rule.CompareDepth; ++level) {
				for (int i = 0; i < TryCount; ++i) {
					var matchingList = Create(_players.GetSource(_rule, level).Where(p => !p.Dropped));
					if (matchingList != null) {
						return matchingList;
					}
				}
			}
			return null;

			IEnumerable<Match> Create(IEnumerable<Player> players) {
				var results = new Queue<Match>();
				var stack = new List<Player>();

				foreach (var p1 in players) {
					var p2 = PickOpponent(stack, p1);
					if (p2 != null) {
						stack.Remove(p2);
						results.Enqueue(new Match(p2, p1));
					} else {
						stack.Add(p1);
					}
				}
				switch (stack.Count) {
					case 0:
						return results;//組み合わせ完了

					case 1:// 1人余り
						{
							var p = stack.First();
							if (AcceptByeMatchDuplication || !p.HasByeMatch) {
								results.Enqueue(new Match(p, _rule));
								return results;//1人不戦勝
							}
						}
						break;
				}
				return null;//組み合わせを作れなかった。

				Player PickOpponent(IEnumerable<Player> opponents, Player player) {
					foreach (var opponent in opponents) {
						if (player.GetResult(opponent) == null) {//対戦履歴なし
							if (AcceptGapMatchDuplication || !player.HasGapMatch) {
								return opponent;//対戦者認定
							}
						}
					}
					return null;//適合者なし
				}
			}
		}
	}
}