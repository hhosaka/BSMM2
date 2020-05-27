using BSMM2.ViewModels;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	internal class SingleMatchLPViewModel : BaseViewModel {
		private bool _player1;

		public bool Player1 {
			get => _player1;
			set {
				if (value != _player1) {
					Draw = Player2 = false;
					SetProperty(ref _player1, value);
				}
			}
		}

		private bool _draw;

		public bool Draw {
			get => _draw;
			set {
				if (value != _draw) {
					Player1 = Player2 = false;
					SetProperty(ref _draw, value);
				}
			}
		}

		private bool _player2;

		public bool Player2 {
			get => _player2;
			set {
				if (value != _player2) {
					Draw = Player1 = false;
					SetProperty(ref _player2, value);
				}
			}
		}

		public int Player1LP { get; set; }
		public int Player2LP { get; set; }

		public ICommand DoneCommand { get; }

		public SingleMatchLPViewModel(SingleMatchRule rule, Match match, Action back) {
			DoneCommand = new Command(Done);
			var record1 = match.Record1;
			var record2 = match.Record2;

			switch (match.GetResult()) {
				case RESULT_T.Win:
					Player1 = true;
					break;

				case RESULT_T.Draw:
					Draw = true;
					break;

				case RESULT_T.Lose:
					Player2 = true;
					break;
			}

			Player1LP = record1.Result.LifePoint;
			Player2LP = record2.Result.LifePoint;

			void Done() {
				if (GetResult() != RESULT_T.Progress) {
					match.SetResults(rule.CreatePoints(GetResult(), Player1LP, Player2LP));
					MessagingCenter.Send<object>(this, Messages.REFRESH);
				}
				back?.Invoke();
			}

			RESULT_T GetResult() {
				if (Player1)
					return RESULT_T.Win;
				else if (Player2) {
					return RESULT_T.Lose;
				} else if (Draw) {
					return RESULT_T.Draw;
				} else {
					return RESULT_T.Progress;
				}
			}
		}
	}
}