﻿using BSMM2.Models.Matches;
using BSMM2.Models.Matches.SingleMatch;
using BSMM2.Services;
using Newtonsoft.Json;
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
		private static readonly string APPDATAPATH = "appfile.data";

		[JsonIgnore]
		private static readonly IGame _defaultGame = new DefaultGame();

		public static BSMMApp Create(bool force = false) {
			var engine = new SerializeUtil();

			if (force) {
				return Initiate();
			} else {
				try {
					return engine.Load<BSMMApp>(APPDATAPATH, Initiate);
				} catch (IOException) {
					return Initiate();
				}
			}

			BSMMApp Initiate()
				=> new BSMMApp(engine,
						new Rule[] {
					new SingleMatchRule(),
					new ThreeGameMatchRule(),
					new ThreeOnThreeMatchRule(),
						});
		}

		[JsonIgnore]
		private SerializeUtil _engine;

		[JsonProperty]
		private List<IGame> _games;

		[JsonIgnore]
		public IEnumerable<IGame> Games => _games;

		[JsonProperty]
		public IEnumerable<Rule> Rules { get; private set; }

		[JsonProperty]
		public Rule Rule { get; set; }

		[JsonProperty]
		public IGame Game { get; private set; }

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

		private BSMMApp(SerializeUtil engine, Rule[] rules) : this(engine) {
			Rules = rules;
			Game = _defaultGame;
			Rule = Rules.First();
			_games = new List<IGame>();
			AutoSave = true;
		}

		public bool Add(IGame game, bool AsCurrentGame) {
			if (Game != _defaultGame && AsCurrentGame) {
				Remove(Game);
			}
			_games.Add(game);
			Game = game;
			return true;
		}

		public bool Remove(IGame game) {
			_games.Remove(game);
			Game = _defaultGame;
			return true;
		}

		public bool Select(IGame game) {
			Game = game;
			return true;
		}

		public void Save(bool force) {
			if (force || AutoSave)
				_engine.Save(this, APPDATAPATH);
		}

		public ContentPage CreateMatchPage(Match match) {
			return Game.CreateMatchPage(match);
		}

		public async Task SendEmail(string subject, string body, IEnumerable<string> recipients) {
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

		public void SendByMail(Game game, string to, string filename) {
			var buf = new StringBuilder();
			using (var writer = new StringWriter(buf)) {
				new Serializer<Game>().Serialize(writer, game);
				SendEmail("BS Match Maker Result", buf.ToString(), new[] { MailAddress }).Start();
			}
		}

		public async void Export() {
			var buf = new StringBuilder();
			Game.Players.Export(new StringWriter(buf));
			await SendEmail(Game.Headline, buf.ToString(), new[] { MailAddress });
		}

		// TODO : TBD
		//public DelegateCommand CreateStartCommand(Action onChanged)
		//	=> new DelegateCommand(() => { Game.StepToPlaying(); onChanged(); }, () => Game.CanExecuteStepToPlaying);
	}
}