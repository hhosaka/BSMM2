using BSMM2.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayersPage : ContentPage {
		private PlayersViewModel viewModel;

		public PlayersPage() {
			InitializeComponent();

			BindingContext = viewModel = new PlayersViewModel();
		}

		private async void OnPlayerSelected(object sender, SelectedItemChangedEventArgs args) {
			//	var player = args.SelectedItem as Player;
			//	if (player == null)
			//		return;

			//	await Navigation.PushAsync(new PlayerDetailPage(new PlayerDetailViewModel(player)));

			//	// Manually deselect item.
			//	PlayersListView.SelectedItem = null;
		}

		private async void NewGame_Clicked(object sender, EventArgs e) {
			await Navigation.PushModalAsync(new NavigationPage(new NewGamePage()));
		}

		protected override void OnAppearing() {
			base.OnAppearing();

			if (viewModel.Players.Count == 0)
				viewModel.LoadPlayersCommand.Execute(null);
		}
	}
}