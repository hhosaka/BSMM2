﻿using BSMM2.ViewModels;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace BSMM2.Models.Matches.SingleMatch {

	internal class SingleMatchViewModel : BaseViewModel {

		internal class Item {
			public string Name { get; }
			public RESULT_T RESULT { get; }

			public Item(string name, RESULT_T result) {
				Name = name;
				RESULT = result;
			}
		}

		private IMatch _match;

		public SingleMatchRule Rule { get; }
		public bool EnableLifePoint { get; }
		public int LifePoint1 { get; set; }
		public int LifePoint2 { get; set; }
		public string LifePointTitle1 { get; }
		public string LifePointTitle2 { get; }
		public Item SelectedItem { get; set; }

		public int[] LifePoints { get; }
		public ObservableCollection<Item> Items { get; }

		public bool Update() {
			if (SelectedItem != null) {
				_match.SetResults(Rule.CreatePoints(SelectedItem.RESULT, LifePoint1, LifePoint2));
				MessagingCenter.Send<object>(this, "UpdatedMatch");
				return true;
			}
			return false;
		}

		public SingleMatchViewModel(Game game, IMatch match) {
			Rule = game.Rule as SingleMatchRule;
			EnableLifePoint = game.EnableLifePoint;
			_match = match;
			var record1 = match.Record1;
			var record2 = match.Record2;
			var items = new ObservableCollection<Item>();
			items.Add(new Item(record1.Player.Name + " Win", RESULT_T.Win));
			items.Add(new Item("Draw", RESULT_T.Draw));
			items.Add(new Item(record2.Player.Name + " Win", RESULT_T.Lose));
			Items = items;

			LifePoint1 = record1.Result?.LifePoint ?? 0;
			LifePointTitle1 = record1.Player.Name + "'s remaining Life Point";
			LifePoint2 = record2.Result?.LifePoint ?? 0;
			LifePointTitle2 = record2.Player.Name + "'s remaining Life Point";
			LifePoints = new[] { 0, 1, 2, 3, 4, 5 };
		}
	}
}