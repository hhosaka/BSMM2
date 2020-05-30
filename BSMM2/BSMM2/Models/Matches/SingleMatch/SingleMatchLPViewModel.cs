using BSMM2.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	using LifePoints = IEnumerable<LifePoint>;

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