using BSMM2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	using LifePoints = IEnumerable<LifePoint>;

	internal class LifePoint {
		private static LifePoints _instance;

		public string Label { get; private set; }
		public int? Point { get; private set; }

		public static LifePoints Instance => GetInstance();

		private static LifePoints GetInstance() {
			if (_instance == null) {
				_instance = new List<LifePoint> {
					new LifePoint{ Label = "-", Point = null},
					new LifePoint{ Label = "5", Point = 5},
					new LifePoint{ Label = "4", Point = 4},
					new LifePoint{ Label = "3", Point = 3},
					new LifePoint{ Label = "2", Point = 2},
					new LifePoint{ Label = "1", Point = 1},
					new LifePoint{ Label = "0", Point = 0},
				};
			}
			return _instance;
		}

		public static LifePoint GetItem(int? point)
			=> GetInstance().First(lp => lp.Point == point);
	}

	internal class ResultItem {
		private Action _onPropertyChanged;

		public RESULT_T Value { get; private set; }

		public ResultItem(Match match, Action onPropertyChanged) {
			Initial(match.GetResult());
			_onPropertyChanged = onPropertyChanged;
		}

		public bool Player1Win {
			get => Value == RESULT_T.Win;
			set {
				if (value != Player1Win) {
					if (value) {
						Draw = Player2Win = false;
						Value = RESULT_T.Win;
					} else {
						Value = RESULT_T.Progress;
					}
					_onPropertyChanged?.Invoke();
				}
			}
		}

		public bool Draw {
			get => Value == RESULT_T.Draw;
			set {
				if (value != Draw) {
					if (value) {
						Player1Win = Player2Win = false;
						Value = RESULT_T.Draw;
					} else {
						Value = RESULT_T.Progress;
					}
					_onPropertyChanged?.Invoke();
				}
			}
		}

		public bool Player2Win {
			get => Value == RESULT_T.Lose;
			set {
				if (value != Player2Win) {
					if (value) {
						Draw = Player1Win = false;
						Value = RESULT_T.Lose;
					} else {
						Value = RESULT_T.Progress;
					}
					_onPropertyChanged?.Invoke();
				}
			}
		}

		private void Initial(RESULT_T result) {
			switch (result) {
				case RESULT_T.Win:
					Player1Win = true;
					break;

				case RESULT_T.Draw:
					Draw = true;
					break;

				case RESULT_T.Lose:
					Player2Win = true;
					break;
			}
		}
	}

	internal class SingleMatchLPViewModel : BaseViewModel {
		public ResultItem ResultItem { get; }

		private Match _match;

		public LifePoint Player1LP { get; set; }
		public LifePoint Player2LP { get; set; }
		public IPlayer Player1 => _match.Record1.Player;
		public IPlayer Player2 => _match.Record2.Player;

		public LifePoints LifePoints
			=> LifePoint.Instance;

		public ICommand DoneCommand { get; }

		public SingleMatchLPViewModel(SingleMatchRule rule, Match match, Action back) {
			DoneCommand = new Command(Done);

			_match = match;

			Player1LP = LifePoint.GetItem(match.Record1.Result.LifePoint);
			Player2LP = LifePoint.GetItem(match.Record2.Result.LifePoint);

			ResultItem = new ResultItem(match, () => OnPropertyChanged(nameof(ResultItem)));

			void Done() {
				if (ResultItem.Value != RESULT_T.Progress) {
					match.SetResults(rule.CreatePoints(ResultItem.Value, Player1LP.Point ?? 0, Player2LP.Point ?? 0));// TODO tentativce
					MessagingCenter.Send<object>(this, Messages.REFRESH);
				}
				back?.Invoke();
			}
		}
	}
}