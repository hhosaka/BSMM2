using BSMM2.Models.Matches;
using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public class BSMMApp {

		public static BSMMApp Create(string path, bool force) {
			var engine = new SerializeUtil();

			if (force) {
				return Initiate();
			} else {
				try {
					return engine.Load<BSMMApp>(path, Initiate);
				} catch (Exception) {
					return Initiate();
				}
			}

			BSMMApp Initiate()
				=> new BSMMApp(engine,
						path,
						new Rule[] {
					new SingleMatchRule(),
					new ThreeGameMatchRule(),
					new ThreeOnThreeMatchRule(),
						});
		}

		[JsonProperty]
		private string _path;

		[JsonIgnore]
		private SerializeUtil _engine;

		[JsonProperty]
		private List<Game> _games;

		[JsonIgnore]
		public IEnumerable<Game> Games => _games;

		[JsonProperty]
		public IEnumerable<Rule> Rules { get; private set; }

		[JsonProperty]
		public Rule Rule { get; set; }

		[JsonProperty]
		public Game Game { get; private set; }

		[JsonProperty]
		public bool AutoSave { get; set; }

		[JsonProperty]
		public string MailAddress { get; set; }

		public BSMMApp() : this(new SerializeUtil()) {// for Serializer
		}

		private BSMMApp(SerializeUtil engine) {
			_engine = engine;
			MessagingCenter.Subscribe<object>(this, Messages.REFRESH, (sender) => Save(false));
		}

		private BSMMApp(SerializeUtil engine, string path, Rule[] rules) : this(engine) {
			Rules = rules;
			_path = path;
			Rule = Rules.First();
			_games = new List<Game>() { new Game(rules[0], new Players(rules[0], 8)) };
			Game = _games.Last();
			AutoSave = true;
		}

		public bool Add(Game game, bool AsCurrentGame) {
			if (AsCurrentGame) {
				_games.Remove(Game);
			}
			_games.Add(game);
			Game = game;
			return true;
		}

		public bool Remove(Game game) {
			if (_games.Count > 1) {
				_games.Remove(game);
				Game = _games.Last();
				return true;
			}
			return false;
		}

		public bool Select(Game game) {
			Game = game;
			return true;
		}

		public void Save(bool force) {
			if (force || AutoSave)
				_engine.Save(this, _path);
		}

		public ContentPage CreateMatchPage(Match match) {
			return Game.CreateMatchPage(match);
		}

		public async Task SendByMail(string subject, string body) {
			await SendByMail(subject, body, new[] { MailAddress });
		}

		public async Task SendByMail(string subject, string body, IEnumerable<string> recipients) {
			try {
				var message = new EmailMessage {
					Subject = subject,
					Body = body,
					To = recipients.ToList(),
				};
				await Email.ComposeAsync(message);
			} catch (FeatureNotSupportedException) {
				// Email is not supported on this device
			}
		}

		public async void ExportPlayers() {
			var buf = new StringBuilder();
			Game.Players.Export(new StringWriter(buf));
			await SendByMail(Game.Headline, buf.ToString(), new[] { MailAddress });
		}

		// TODO : TBD
		//public DelegateCommand CreateStartCommand(Action onChanged)
		//	=> new DelegateCommand(() => { Game.StepToPlaying(); onChanged(); }, () => Game.CanExecuteStepToPlaying);
	}
}