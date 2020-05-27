using System.Collections.ObjectModel;

namespace BSMM2.Models.Matches {

	using LIFEPOINTITEM_T = System.Collections.Generic.KeyValuePair<string, int?>;
	using RESULTITEM_T = System.Collections.Generic.KeyValuePair<string, RESULT_T>;

	internal class ControlUtil {

		public static ObservableCollection<RESULTITEM_T> CreateResultSelector(string player1, string player2) {
			var items = new ObservableCollection<RESULTITEM_T>();
			items.Add(new RESULTITEM_T(player1 + " Win", RESULT_T.Win));
			items.Add(new RESULTITEM_T("Draw", RESULT_T.Draw));
			items.Add(new RESULTITEM_T(player2 + " Win", RESULT_T.Lose));
			return items;
		}

		public static ObservableCollection<LIFEPOINTITEM_T> CreateLifePointItems() {
			var items = new ObservableCollection<LIFEPOINTITEM_T>();
			items.Add(new LIFEPOINTITEM_T("Undefined", null));
			for (int i = 5; i >= 0; --i) {
				items.Add(new LIFEPOINTITEM_T(i.ToString(), i));
			}
			return items;
		}
	}
}