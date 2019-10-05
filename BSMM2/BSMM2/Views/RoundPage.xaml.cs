using BSMM2.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoundPage : ContentPage {
		private RoundViewModel viewModel;

		public RoundPage() {
			InitializeComponent();

			BindingContext = viewModel = new RoundViewModel();
		}

		private async void OnMatchTapped(object sender, SelectedItemChangedEventArgs args) {
			//	var player = args.SelectedItem as Player;
			//	if (player == null)
			//		return;

			//	await Navigation.PushAsync(new PlayerDetailPage(new PlayerDetailViewModel(player)));

			// Manually deselect item.
			RoundListView.SelectedItem = null;
		}

		private async void NewGame_Clicked(object sender, EventArgs e) {
			await Navigation.PushModalAsync(new NavigationPage(new NewGamePage()));
		}

		protected override void OnAppearing() {
			base.OnAppearing();

			if (viewModel.Matches.Count == 0)
				viewModel.LoadMatchesCommand.Execute(null);
		}
	}
}