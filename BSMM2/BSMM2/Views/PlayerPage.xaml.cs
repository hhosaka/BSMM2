using BSMM2.Models;
using BSMM2.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayerPage : ContentPage {

		public PlayerPage(BSMMApp app, Player player) {
			InitializeComponent();
			BindingContext = new PlayerViewModel(app, player);

			Title = player.Name;

			var fontSize = (grid.Children[0] as Label).FontSize;

			int i = 1;
			foreach (var param in player.Export(new Dictionary<string, string>())) {
				CreateLabel(i, 0, param.Key);
				CreateLabel(i++, 1, param.Value);
				grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			}

			void CreateLabel(int row, int col, string text) {
				var label = new Label() { Text = text, FontSize = fontSize, BackgroundColor = row % 2 == 1 ? Color.LightGray : Color.White };
				grid.Children.Add(label);
				Grid.SetColumn(label, col);
				Grid.SetRow(label, row);
			}
		}

		private async void Back(object sender, EventArgs e)
			=> await Navigation.PopModalAsync();

		private void OnClosing(object sender, EventArgs e)
			=> MessagingCenter.Send<object>(this, Messages.REFRESH);
	}
}