using BSMM2.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewGamePage : ContentPage {
		private NewGameViewModel _viewModel;

		public NewGamePage() {
			InitializeComponent();
			BindingContext = _viewModel = new NewGameViewModel();
		}

		private async void NewGame_Clicked(object sender, EventArgs e) {
			_viewModel.ExecuteNewGame();
			MessagingCenter.Send(this, "NewGame");
			await Navigation.PopModalAsync();
		}
	}
}