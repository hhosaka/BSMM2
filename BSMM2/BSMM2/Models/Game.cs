using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public class Game {

		[JsonProperty]
		public string Title { get; set; }

		[JsonProperty]
		public Guid Id { get; }

		[JsonProperty]
		public bool EnableLifePoint { get; }

		[JsonProperty]
		public Rule Rule { get; private set; }

		[JsonProperty]
		private readonly Players _players;

		[JsonProperty]
		private readonly Stack<IRound> _rounds;

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
		public IEnumerable<IRound> Rounds
			=> _rounds;

		[JsonIgnore]
		public IRound ActiveRound => _activeRound;

		[JsonIgnore]
		public bool Locked => (_activeRound as Matching)?.Locked == true;

		public ContentPage CreateMatchPage(IMatch match) {
			return Rule.CreateMatchPage(this, match);
		}

		public Game() {// For Serializer
		}

		public Game(Rule rule, Players players, bool enableLifePoint, string title = null) {
			EnableLifePoint = enableLifePoint;
			Title = title ?? DateTime.Now.ToString();
			Id = Guid.NewGuid();
			_players = players;
			Rule = rule;
			_rounds = new Stack<IRound>();
			_startTime = null;
			var result = CreateMatching();
			Debug.Assert(result);
		}

		private bool CreateMatching() {
			var round = MakeRound();
			if (round != null) {
				_activeRound = new Matching(MakeRound());
				return true;
			}
			return false;
		}

		public bool CanExecuteShuffle()
			=> (_activeRound as Matching)?.Locked == false;

		public bool Shuffle() {
			if (CanExecuteShuffle()) {
				return CreateMatching();
			}
			return false;
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

		public bool StepToMatching() {
			if (CanExecuteStepToMatching()) {
				_startTime = null;
				var round = _activeRound;
				if (CreateMatching()) {
					_rounds.Push(round);
					return true;
				}
			}
			return false;
		}

		public bool IsMatching()
			=> _activeRound is Matching;

		private IEnumerable<Player> GetOrderedPlayers() {
			var comparer = Rule.CreateOrderComparer();
			var players = _players.GetByOrder(Rule);
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

		[JsonIgnore]
		public IEnumerable<Player> PlayersByOrder
				=> GetOrderedPlayers();

		private IEnumerable<Match> MakeRound() {
			_players.Reset(Rule);
			for (int level = 0; level < Rule.CompareDepth; ++level) {
				for (int i = 0; i < TryCount; ++i) {
					var matchingList = Create(_players.GetSource(Rule, level).Where(p => !p.Dropped));
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
							if (isByeAcceptable(p)) {
								results.Enqueue(new Match(p, Rule));
								return results;//1人不戦勝
							}
						}
						break;
				}
				return null;//組み合わせを作れなかった。

				bool isByeAcceptable(Player p) {
					if (p.IsAllWins) return false;
					return p.IsAllLoses || (AcceptByeMatchDuplication || !p.HasByeMatch);
				}

				Player PickOpponent(IEnumerable<Player> opponents, Player player) {
					foreach (var opponent in opponents) {
						if (player.GetResult(opponent) == null) {//対戦履歴なし
							if (CheckGapMatch()) {
								return opponent;//対戦者認定
							}
						}

						bool CheckGapMatch() {// 階段戦の重複は避ける
							if (!AcceptGapMatchDuplication) {
								if (player.Result.Point != opponent.Result.Point) {
									return !player.HasGapMatch && !opponent.HasGapMatch;
								}
							}
							return true;
						}
					}
					return null;//適合者なし
				}
			}
		}
	}
}