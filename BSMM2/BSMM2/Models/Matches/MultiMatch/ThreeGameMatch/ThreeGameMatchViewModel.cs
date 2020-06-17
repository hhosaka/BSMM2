using BSMM2.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static BSMM2.Models.Matches.MultiMatch.MultiMatch;

namespace BSMM2.Models.Matches.MultiMatch.ThreeGameMatch {

	internal class TheConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is ResultItem resultItem)
				return resultItem.Value != RESULT_T.Progress;

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}

	internal class ThreeGameMatchViewModel : BaseViewModel {
		private MultiMatch _match;
		private ThreeGameMatchRule _rule;

		public bool EnableLifePoint => _rule.EnableLifePoint;

		//private ResultItem[] _resultItems;

		public ResultItem[] ResultItems { get; }

		public LifePoint[] Player1LPs { get; }
		public LifePoint[] Player2LPs { get; }
		public IPlayer Player1 => _match.Record1.Player;
		public IPlayer Player2 => _match.Record2.Player;

		public ICommand DoneCommand { get; }

		public ThreeGameMatchViewModel(ThreeGameMatchRule rule, MultiMatch match, Action back) {
			DoneCommand = new Command(Done);

			_match = match;
			_rule = rule;
			if (match.Record1.Result is MultiMatchResult result1) {
				ResultItems = CreateItems();
				Player1LPs = CreateLifePoints(result1);
				Player2LPs = CreateLifePoints((MultiMatchResult)match.Record2.Result);
			} else {
				var item = new ResultItem(RESULT_T.Progress, () => OnPropertyChanged(nameof(ResultItems)));
				ResultItems = new[] { item, item, item };
				Player1LPs = Player2LPs = new[] { LifePoint.GetItem(-1), LifePoint.GetItem(-1), LifePoint.GetItem(-1) };
			}

			ResultItem[] CreateItems() {
				var items = new List<ResultItem>();
				result1.Results.ForEach(result => items.Add(new ResultItem(result.RESULT, () => OnPropertyChanged(nameof(ResultItems)))));
				return items.ToArray();
			}

			LifePoint[] CreateLifePoints(MultiMatchResult results) {
				var buf = new List<LifePoint>();
				results.Results.ForEach(result => buf.Add(LifePoint.GetItem(result.LifePoint)));
				return buf.ToArray();
			}
			void Done() {
				match.SetMultiMatchResult(new[] {
					new Score(ResultItems[0].Value, Player1LPs[0].Point, Player2LPs[0].Point),
					new Score(ResultItems[1].Value, Player1LPs[1].Point, Player2LPs[1].Point),
					new Score(ResultItems[2].Value, Player1LPs[2].Point, Player2LPs[2].Point),
				}, EnableLifePoint);
				MessagingCenter.Send<object>(this, Messages.REFRESH);
				back?.Invoke();
			}
		}
	}
}