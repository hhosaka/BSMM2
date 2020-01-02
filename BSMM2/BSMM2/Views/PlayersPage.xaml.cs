using BSMM2.Models;
using BSMM2.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayersPage : ContentPage {
		private PlayersViewModel viewModel;
		private BSMMApp _app;

		public PlayersPage(BSMMApp app) {
			_app = app;
			InitializeComponent();

			BindingContext = viewModel = new PlayersViewModel(app);
		}

		private async void OnPlayerTapped(object sender, SelectedItemChangedEventArgs args) {
			//	var player = args.SelectedItem as Player;
			//	if (player == null)
			//		return;

			//	await Navigation.PushAsync(new PlayerDetailPage(new PlayerDetailViewModel(player)));

			// Manually deselect item.
			PlayersListView.SelectedItem = null;
		}

		private async void NewGame_Clicked(object sender, EventArgs e) {
			await Navigation.PushModalAsync(new NavigationPage(new NewGamePage(_app)));
		}

		private async void RemoveGame_Clicked(object sender, EventArgs e) {
			var accepted = await DisplayAlert(
				  "Delete Current Game", "Press Done to delete current Game", "Done", "Cancel");
			if (accepted) {
				_app.RemoveGame();
				MessagingCenter.Send<object>(this, "UpdatedRound");
			};
		}

		private async void SelectGame_Clicked(object sender, EventArgs e) {
			// TBD
		}

		private async void AddPlayer_Clicked(object sender, EventArgs e) {
			// TBD
		}
	}
}