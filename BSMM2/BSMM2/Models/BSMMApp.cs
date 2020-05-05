using BSMM2.Models.Matches;
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

		public static BSMMApp Create() {
			var engine = new Engine();
			return engine.Load<BSMMApp>(APPDATAPATH, Initiate);

			BSMMApp Initiate()
				=> new BSMMApp(engine,
						new Rule[] {
					new SingleMatchRule(),
					new ThreeGameMatchRule(),
					new ThreeOnThreeMatchRule(),
						});
		}

		[JsonIgnore]
		private Engine _engine;

		[JsonProperty]
		private List<IGame> _games;

		[JsonIgnore]
		public IEnumerable<IGame> Games => _games;

		[JsonProperty]
		public IEnumerable<Rule> Rules { get; private set; }

		[JsonProperty]
		public Rule Rule { get; set; }

		[JsonProperty]
		private IGame _game;

		[JsonIgnore]
		public IGame Game => _game ?? _defaultGame;

		[JsonProperty]
		public bool AutoSave { get; set; }

		public BSMMApp() : this(new Engine()) {// for Serializer
		}

		private BSMMApp(Engine engine) {
			_engine = engine;
			MessagingCenter.Subscribe<object>(this, Messages.REFRESH, (sender) => Save(false));
		}

		private BSMMApp(Engine engine, Rule[] rules) : this(engine) {
			Rules = rules;
			Rule = Rules.First();
			_games = new List<IGame>();
			AutoSave = true;
		}

		public bool Add(IGame game, bool AsCurrentGame) {
			if (_game != null && AsCurrentGame) {
				Remove(_game);
			}
			_games.Add(game);
			_game = game;
			return true;
		}

		public bool Remove(IGame game) {
			_games.Remove(game);
			_game = null;
			return true;
		}

		public bool Select(IGame game) {
			_game = game;
			return true;
		}

		public void Save(bool force) {
			if (force || AutoSave)
				_engine.Save(this, APPDATAPATH);
		}

		public ContentPage CreateMatchPage(Match match) {
			return Game.CreateMatchPage(match);
		}

		public async Task SendEmail(string subject, string body, List<string> recipients) {
			try {
				var message = new EmailMessage {
					Subject = subject,
					Body = body,
					To = recipients,
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
				SendEmail("BS Match Maker Result", buf.ToString(), new[] { "hhosaka183@gmail.com" }.ToList()).Start();
			}
		}

		// TODO : TBD
		//public DelegateCommand CreateStartCommand(Action onChanged)
		//	=> new DelegateCommand(() => { Game.StepToPlaying(); onChanged(); }, () => Game.CanExecuteStepToPlaying);
	}
}