using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public class Game {
		private static readonly int _tryCount = 100;

		public static string GenerateTitle()
			=> "Game" + DateTime.Now.ToString();

		[JsonProperty]
		public string Title { get; private set; }

		[JsonProperty]
		public Guid Id { get; private set; }

		[JsonProperty]
		public Rule Rule { get; private set; }

		[JsonProperty]
		public DateTime? StartTime { get; private set; }

		[JsonProperty]
		public Players Players { get; private set; }

		[JsonProperty]
		public List<IRound> _rounds;

		[JsonIgnore]
		public IEnumerable<IRound> Rounds
			=> _rounds;

		[JsonProperty]
		public IRound ActiveRound { get; private set; }

		[JsonIgnore]
		public string Headline => Title + "(Round " + (Rounds?.Count() + 1 ?? 0) + ")";

		[JsonIgnore]
		public bool CanAddPlayers => true;//TODO: tentative

		public bool AddPlayers(string data) {
			foreach (var name in data.Split(new[] { '\r', '\n' })) {
				if (!string.IsNullOrEmpty(name)) {
					Players.Add(name);
					//AddPlayer(name);
				}
			}
			Shuffle();
			return true;
		}

		public Game() {// For Serializer
		}

		public Game(Rule rule, Players players, string title = null) {
			if (string.IsNullOrEmpty(title))
				Title = GenerateTitle();
			else
				Title = title;

			Id = Guid.NewGuid();
			Players = players;
			Rule = rule;
			_rounds = new List<IRound>();
			StartTime = null;
			var result = CreateMatching();
			Debug.Assert(result);
		}

		public ContentPage CreateMatchPage(IMatch match) {
			return Rule.CreateMatchPage(this, match);
		}

		private bool CreateMatching() {
			var round = MakeRound();
			if (round != null) {
				ActiveRound = new Matching(MakeRound());
				return true;
			}
			return false;
		}

		[JsonIgnore]
		public bool CanExecuteShuffle
			=> ActiveRound is Matching;

		public bool Shuffle() {
			if (CanExecuteShuffle) {
				return CreateMatching();
			}
			return false;
		}

		[JsonIgnore]
		public bool CanExecuteStepToPlaying
			=> ActiveRound is Matching;

		public void StepToPlaying() {
			if (CanExecuteStepToPlaying) {
				ActiveRound = new Round(ActiveRound);
				StartTime = DateTime.Now;
			}
		}

		[JsonIgnore]
		public bool CanExecuteStepToMatching
			=> (ActiveRound as Round)?.IsFinished == true;

		public bool StepToMatching() {
			if (CanExecuteStepToMatching) {
				StartTime = null;
				var round = ActiveRound;
				if (CreateMatching()) {
					_rounds.Add(round);
					return true;
				}
			}
			return false;
		}

		[JsonIgnore]
		public bool IsMatching
			=> ActiveRound is Matching;

		private IEnumerable<Match> MakeRound() {
			Players.Reset();
			for (int level = 0; level < Rule.CompareDepth; ++level) {
				for (int i = 0; i < _tryCount; ++i) {
					var matchingList = Create(Players.GetSource(Rule, level).Where(p => !p.Dropped));
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
					return p.IsAllLoses || (Rule.AcceptByeMatchDuplication || !p.HasByeMatch);
				}

				Player PickOpponent(IEnumerable<Player> opponents, Player player) {
					foreach (var opponent in opponents) {
						if (player.GetResult(opponent) == null) {//対戦履歴なし
							if (CheckGapMatch()) {
								return opponent;//対戦者認定
							}
						}

						bool CheckGapMatch() {// 階段戦の重複は避ける
							if (!Rule.AcceptGapMatchDuplication) {
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

		public static Game Load(Guid id, SerializeUtil engine) {
			return engine.Load<Game>(id.ToString() + ".json", () => new Game());
		}

		public void Save(SerializeUtil engine) {
			engine.Save<Game>(this, Id.ToString() + ".json");
		}

		public void Remove(SerializeUtil engine, Game game) {
			engine.Delete(game.Id.ToString() + ".json");
		}
	}
}