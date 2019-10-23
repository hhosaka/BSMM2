using BSMM2.Models;
using BSMM2.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewGamePage : ContentPage {
		private NewGameViewModel _viewModel;

		public NewGamePage(BSMMApp app) {
			InitializeComponent();
			BindingContext = _viewModel = new NewGameViewModel(app);
		}

		private async void NewGame_Clicked(object sender, EventArgs e) {
			if (_viewModel.ExecuteNewGame())
				await Navigation.PopModalAsync();
		}
	}
}