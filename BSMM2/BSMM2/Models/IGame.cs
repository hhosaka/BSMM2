using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BSMM2.Models {

	public interface IGame {
		Guid Id { get; }
		Rule Rule { get; }
		IRound ActiveRound { get; }
		IEnumerable<IRound> Rounds { get; }
		DateTime? StartTime { get; }
		bool IsMatching { get; }
		string Headline { get; }
		IEnumerable<Player> PlayersByOrder { get; }

		bool CanExecuteShuffle { get; }

		bool Shuffle();

		bool CanExecuteStepToPlaying { get; }

		void StepToPlaying();

		bool CanExecuteStepToMatching { get; }

		bool StepToMatching();

		ContentPage CreateMatchPage(IMatch match);

		bool AddPlayers(string data);

		bool CanAddPlayers { get; }
	}
}