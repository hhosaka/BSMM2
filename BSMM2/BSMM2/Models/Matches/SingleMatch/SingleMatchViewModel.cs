using BSMM2.ViewModels;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	internal class SingleMatchViewModel : BaseViewModel {

		internal class Item {
			public string Label { get; }
			public RESULT_T RESULT { get; }

			public Item(string label, RESULT_T result) {
				Label = label;
				RESULT = result;
			}
		}

		private Match _match;

		public bool EnableLifePoint { get; }
		public int LifePoint1 { get; set; }
		public int LifePoint2 { get; set; }
		public string LifePointTitle1 { get; }
		public string LifePointTitle2 { get; }

		private Action _update;

		private Item _selectedItem;

		public Item SelectedItem {
			get => _selectedItem;
			set {
				SetProperty(ref _selectedItem, value);
				_update?.Invoke();
			}
		}

		public ObservableCollection<Item> Items { get; }

		public SingleMatchViewModel(SingleMatchRule rule, Match match, Action back) {
			_match = match;
			EnableLifePoint = rule.EnableLifePoint;
			var record1 = match.Record1;
			var record2 = match.Record2;
			var items = new ObservableCollection<Item>();
			items.Add(new Item(record1.Player.Name + " Win", RESULT_T.Win));
			items.Add(new Item("Draw", RESULT_T.Draw));
			items.Add(new Item(record2.Player.Name + " Win", RESULT_T.Lose));
			Items = items;
			_update = Update;

			LifePoint1 = record1.Result?.LifePoint ?? 0;
			LifePointTitle1 = String.Format("{0}'s remaining Life Point", record1.Player.Name);
			LifePoint2 = record2.Result?.LifePoint ?? 0;
			LifePointTitle2 = String.Format("{0}'s remaining Life Point", record2.Player.Name);

			void Update() {
				if (SelectedItem != null) {
					_match.SetResults(rule.CreatePoints(SelectedItem.RESULT, LifePoint1, LifePoint2));
					MessagingCenter.Send<object>(this, Messages.REFRESH);
					back?.Invoke();
				}
			}
		}
	}
}