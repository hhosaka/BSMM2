using BSMM2.Models;
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
			if (args.SelectedItem is Match match)
				await Navigation.PushAsync(new MatchPage(viewModel.Game._rule, match));

			// Manually deselect item.
			RoundListView.SelectedItem = null;
		}

		private async void NewGame_Clicked(object sender, EventArgs e) {
			await Navigation.PushModalAsync(new NavigationPage(new NewGamePage()));
		}

		protected override void OnAppearing() {
			base.OnAppearing();

			if (viewModel.Matches.Count == 0)
				viewModel.LoadRoundCommand.Execute(null);
		}
	}
}