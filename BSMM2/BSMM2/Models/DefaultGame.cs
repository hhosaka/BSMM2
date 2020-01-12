using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BSMM2.Models {

	internal class DefaultGame : IGame {
		private IRound _defaultRound = new Matching(Enumerable.Empty<Match>());

		public IEnumerable<Player> PlayersByOrder
			=> Enumerable.Empty<Player>();

		public IRound ActiveRound
			=> _defaultRound;

		public IEnumerable<IRound> Rounds
			=> Enumerable.Empty<IRound>();

		public string Headline => "Create New Game";
		public DateTime? StartTime => null;

		public bool CanExecuteShuffle
			=> false;

		public bool CanExecuteStepToMatching
			=> false;

		public bool CanExecuteStepToPlaying
			=> false;

		public ContentPage CreateMatchPage(IMatch match)
			=> null;//TBD

		public bool IsMatching
			=> true;

		public Guid Id => Guid.Empty;

		public bool Shuffle()
			=> false;

		public bool StepToMatching()
			=> false;

		public void StepToPlaying() {
		}
	}
}