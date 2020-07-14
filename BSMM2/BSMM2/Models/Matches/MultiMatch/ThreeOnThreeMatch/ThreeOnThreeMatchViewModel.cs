using BSMM2.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.Matches.MultiMatch.MultiMatch;

namespace BSMM2.Models.Matches.MultiMatch.ThreeOnThreeMatch {

	internal class ThreeOnThreeMatchViewModel : BaseViewModel {
		private MultiMatch _match;
		private ThreeOnThreeMatchRule _rule;

		public bool EnableLifePoint => _rule.EnableLifePoint;
		public ResultItem[] ResultItem { get; }
		public LifePoint[] Player1LP { get; }
		public LifePoint[] Player2LP { get; }
		public IPlayer Player1 => _match.Record1.Player;
		public IPlayer Player2 => _match.Record2.Player;

		public ICommand DoneCommand { get; }

		public ThreeOnThreeMatchViewModel(ThreeOnThreeMatchRule rule, MultiMatch match, Action back) {
			DoneCommand = new Command(Done);

			_match = match;
			_rule = rule;
			if (match.Record1.Result is MultiMatchResult result1) {
				ResultItem = CreateItems();
				Player1LP = CreateLifePoints(result1);
				Player2LP = CreateLifePoints((MultiMatchResult)match.Record2.Result);
			} else {
				ResultItem = new[] {
					new ResultItem(RESULT_T.Progress, () => OnPropertyChanged(nameof(ResultItem))),
					new ResultItem(RESULT_T.Progress, () => OnPropertyChanged(nameof(ResultItem))),
					new ResultItem(RESULT_T.Progress, () => OnPropertyChanged(nameof(ResultItem))),
				};
				Player1LP = Player2LP = new[]{
					LifePoint.GetItem(-1),
					LifePoint.GetItem(-1),
					LifePoint.GetItem(-1),
				};
			}

			ResultItem[] CreateItems() {
				var items = new List<ResultItem>();
				result1.Results.ForEach(result => items.Add(new ResultItem(result.RESULT, () => OnPropertyChanged(nameof(ResultItem)))));
				return items.ToArray();
			}

			LifePoint[] CreateLifePoints(MultiMatchResult results) {
				var buf = new List<LifePoint>();
				results.Results.ForEach(result => buf.Add(LifePoint.GetItem(result.LifePoint)));
				return buf.ToArray();
			}
			void Done() {
				if (EnableLifePoint) {
					match.SetMultiMatchResult(new[] {
						new Score(ResultItem[0].RESULT, Player1LP[0].Point, Player2LP[0].Point),
						new Score(ResultItem[1].RESULT, Player1LP[1].Point, Player2LP[1].Point),
						new Score(ResultItem[2].RESULT, Player1LP[2].Point, Player2LP[2].Point),
					});
				} else {
					match.SetMultiMatchResult(new[] {
						new Score(ResultItem[0].RESULT),
						new Score(ResultItem[1].RESULT),
						new Score(ResultItem[2].RESULT),
					});
				}
				MessagingCenter.Send<object>(this, Messages.REFRESH);
				back?.Invoke();
			}
		}
	}
}