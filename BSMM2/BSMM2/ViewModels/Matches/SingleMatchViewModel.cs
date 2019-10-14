using BSMM2.Models;
using BSMM2.Models.Matches;
using System.Collections.Generic;

namespace BSMM2.ViewModels.Matches {

	internal class SingleMatchViewModel : BaseViewModel {

		internal class Item {
			public string Name { get; }
			public RESULT_T RESULT { get; }

			public Item(string name, RESULT_T result) {
				Name = name;
				RESULT = result;
			}
		}

		private SingleMatchRule _rule;
		private Match _match;

		public int LifePoint1 { get; set; }
		public int LifePoint2 { get; set; }
		public Item SelectedItem { get; set; }
		public IEnumerable<Item> Items { get; }

		public void Execute() {
			_match.SetResults(_rule.CreatePoints(SelectedItem.RESULT, LifePoint1, LifePoint2));
		}

		public SingleMatchViewModel(SingleMatchRule rule, Match match) {
			_rule = rule;

			var items = new List<Item>();
			items.Add(new Item(match.Player1.Name + " Win", RESULT_T.Win));
			items.Add(new Item("Draw", RESULT_T.Win));
			items.Add(new Item(match.Player2.Name + " Win", RESULT_T.Lose));
			Items = items;
		}
	}
}