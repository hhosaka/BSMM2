using BSMM2.Models;
using BSMM2.Models.Matches;
using Prism.Commands;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

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

		public DelegateCommand DoneCommand { get; }

		public event Action Popup;

		private Match _match;

		public Game Game { get; }
		public int LifePoint1 { get; set; }
		public int LifePoint2 { get; set; }

		private Item _selectedItem;

		public Item SelectedItem {
			get => _selectedItem;
			set {
				if (_selectedItem != value) {
					_selectedItem = value;
					DoneCommand.RaiseCanExecuteChanged();
					OnPropertyChanged(nameof(SelectedItem));
				}
			}
		}

		public IEnumerable<Item> Items { get; }

		public void ExecuteDone() {
			_match.SetResults((Game.Rule as SingleMatchRule).CreatePoints(SelectedItem.RESULT, LifePoint1, LifePoint2));
			MessagingCenter.Send(this, "Update");
			Popup?.Invoke();
		}

		public SingleMatchViewModel(Game game, Match match) {
			Game = game;
			_match = match;
			var items = new List<Item>();
			items.Add(new Item(match.Player1.Name + " Win", RESULT_T.Win));
			items.Add(new Item("Draw", RESULT_T.Win));
			items.Add(new Item(match.Player2.Name + " Win", RESULT_T.Lose));
			Items = items;

			DoneCommand = new DelegateCommand(ExecuteDone, () => SelectedItem != null);
		}
	}
}