using BSMM2.Models;
using BSMM2.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddPlayerPage : ContentPage {
		private GamesViewModel _viewModel;
		private BSMMApp _app;

		public AddPlayerPage(BSMMApp app) {
			_app = app;
			InitializeComponent();
			//_viewModel = new GamesViewModel(app);
			//_viewModel.exit += async () => await Navigation.PopModalAsync();
			BindingContext = new AddPlayerViewModel(_app);
		}

		private async void Back_Clicked(object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}
	}
}