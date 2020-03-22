using BSMM2.Models.Matches;
using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public class BSMMApp {

		[JsonIgnore]
		private static readonly IGame _defaultGame = new DefaultGame();

		public static BSMMApp Create() {
			return new BSMMApp(new Rule[] {
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

		public BSMMApp() {
			_engine = new Engine();
			MessagingCenter.Subscribe<object>(this, Messages.REFRESH, (sender) => _engine.SaveApp(this));
		}

		private BSMMApp(Rule[] rules) : this() {
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
				_engine.SaveApp(this);
		}

		public ContentPage CreateMatchPage(Match match) {
			return Game.CreateMatchPage(match);
		}

		// TODO : TBD
		//public DelegateCommand CreateStartCommand(Action onChanged)
		//	=> new DelegateCommand(() => { Game.StepToPlaying(); onChanged(); }, () => Game.CanExecuteStepToPlaying);
	}
}