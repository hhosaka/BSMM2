using BSMM2.ViewModels;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	using RESULTITEM_T = System.Collections.Generic.KeyValuePair<string, RESULT_T>;

	internal class SingleMatchViewModel : BaseViewModel {
		private Action _update;

		private RESULTITEM_T _selectedItem;

		public RESULTITEM_T SelectedItem {
			get => _selectedItem;
			set {
				SetProperty(ref _selectedItem, value);
				_update.Invoke();
			}
		}

		public ObservableCollection<RESULTITEM_T> Items { get; }

		public SingleMatchViewModel(SingleMatchRule rule, Match match, Action back) {
			_update = Update;
			var record1 = match.Record1;
			var record2 = match.Record2;
			Items = ControlUtil.CreateResultSelector(record1.Player.Name, record2.Player.Name);

			void Update() {
				match.SetResults(rule.CreatePoints(SelectedItem.Value));
				MessagingCenter.Send<object>(this, Messages.REFRESH);
				back?.Invoke();
			}
		}
	}
}