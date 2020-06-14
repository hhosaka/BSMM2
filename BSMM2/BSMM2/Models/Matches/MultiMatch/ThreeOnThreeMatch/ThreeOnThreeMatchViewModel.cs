using BSMM2.Models.Matches.SingleMatch;
using BSMM2.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BSMM2.Models.Matches.MultiMatch.ThreeOnThreeMatch {

	using LifePoints = IEnumerable<LifePoint>;

	internal class ThreeOnThreeMatchViewModel : BaseViewModel {
		private MultiMatch _match;
		private ThreeOnThreeMatchRule _rule;

		public bool EnableLifePoint => _rule.EnableLifePoint;
		public ResultItem[] ResultItem { get; }
		public LifePoint[] Player1LP { get; }
		public LifePoint[] Player2LP { get; }
		public IPlayer Player1 => _match.Record1.Player;
		public IPlayer Player2 => _match.Record2.Player;

		public LifePoints LifePoints
			=> LifePoint.Instance;

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
				Player1LP = new[]{
					LifePoint.GetItem(-1),
					LifePoint.GetItem(-1),
					LifePoint.GetItem(-1),

				};
			}

			ResultItem[] CreateItems() {
				var items = new List<ResultItem>();
				result1.Results.ForEach(result => items.Add(new ResultItem(match.Record1.Result.RESULT, () => OnPropertyChanged(nameof(ResultItem)))));
				return items.ToArray();
			}

			LifePoint[] CreateLifePoints(MultiMatchResult results) {
				var buf = new List<LifePoint>();
				results.Results.ForEach(result => buf.Add(LifePoint.GetItem(result.LifePoint)));
				return buf.ToArray();
			}
			void Done() {
				var result = new MultiMatchResult(rule.MinimumMatchCount);
				result.Add(new SingleMatchResult(ResultItem[0].Value, Player1LP[0].Point));

				//match.SetSingleMatchResult(ResultItem.Value,
				//	EnableLifePoint ? Player1LP.Point : RESULTUtil.DEFAULT_LIFE_POINT,
				//	EnableLifePoint ? Player2LP.Point : RESULTUtil.DEFAULT_LIFE_POINT);
				//MessagingCenter.Send<object>(this, Messages.REFRESH);
				back?.Invoke();
			}
		}
	}
}