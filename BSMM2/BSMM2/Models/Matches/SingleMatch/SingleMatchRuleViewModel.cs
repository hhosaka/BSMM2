using BSMM2.ViewModels;

namespace BSMM2.Models.Matches.SingleMatch {

	internal class SingleMatchRuleViewModel : BaseViewModel {
		public Game Game { get; set; }

		public Rule Rule { get; set; }

		public SingleMatchRuleViewModel(Game game) {
			Game = game;
			Rule = game.Rule;
		}
	}
}