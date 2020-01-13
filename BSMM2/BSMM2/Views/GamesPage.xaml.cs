using BSMM2.Models;
using BSMM2.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static BSMM2.ViewModels.GamesViewModel;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamesPage : ContentPage {
		private GamesViewModel _viewModel;
		private BSMMApp _app;

		public GamesPage(BSMMApp app) {
			_app = app;
			InitializeComponent();
			_viewModel = new GamesViewModel(app, async () => await Navigation.PopModalAsync());
			BindingContext = _viewModel;
		}

		private async void Back_Clicked(object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}

		private async void GamesListView_ItemTapped(object sender, ItemTappedEventArgs e) {
			if (e.Group is Gameset gameset) {
				_viewModel.Select(gameset.Id);
				await Navigation.PopModalAsync();
			}
		}
	}
}