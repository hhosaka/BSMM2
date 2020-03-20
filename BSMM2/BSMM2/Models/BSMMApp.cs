﻿using BSMM2.Models.Matches;
using BSMM2.Models.Matches.SingleMatch;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public class BSMMApp {
		private static readonly IGame _defaultGame = new DefaultGame();

		private Engine _engine;

		[JsonProperty]
		public Dictionary<Guid, string> Games { get; private set; }

		[JsonProperty]
		public IEnumerable<Rule> Rules { get; private set; }

		[JsonProperty]
		public Rule Rule { get; set; }

		[JsonProperty]
		public IGame Game { get; private set; }

		private bool IsValidGame
			=> Game != _defaultGame;

		public BSMMApp() {
			_engine = new Engine();
		}

		public BSMMApp(Engine engine) {
			Rules = new Rule[] {
				new SingleMatchRule(),
				new ThreeGameMatchRule(),
				new ThreeOnThreeMatchRule(),
			};
			Rule = Rules.First();
			Games = new Dictionary<Guid, string>();
			Game = _defaultGame;
			_engine = engine;
		}

		public bool Add(Game game, bool AsCurrentGame) {
			if (IsValidGame && AsCurrentGame && Games.ContainsKey(Game.Id)) {
				Remove(Game.Id);
			}
			Games[game.Id] = game.Title;
			_engine.SaveGame(game);
			Game = game;
			return true;
		}

		public bool Remove(Guid id) {
			Debug.Assert(id != Guid.Empty);
			_engine.RemoveGame(Game);
			Games.Remove(Game.Id);
			Game = _defaultGame;
			return true;
		}

		public Game Select(Guid id) {
			var game = _engine.LoadGame(id);
			if (IsValidGame) {
				Game = game;
			}
			return game;
		}

		public void SaveApp() {
			_engine.SaveApp(this);
		}

		public void Save() {
			if (IsValidGame) {
				_engine.SaveGame(Game);
			}
		}

		public ContentPage CreateMatchPage(Match match) {
			return Game.CreateMatchPage(match);
		}

		public DelegateCommand CreateStartCommand(Action onChanged)
			=> new DelegateCommand(() => { Game.StepToPlaying(); onChanged(); }, () => Game.CanExecuteStepToPlaying);
	}
}