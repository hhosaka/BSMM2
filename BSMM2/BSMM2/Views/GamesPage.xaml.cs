﻿using BSMM2.Models;
using BSMM2.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static BSMM2.ViewModels.GamesViewModel;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GamesPage : ContentPage {
		private GamesViewModel _viewModel;

		public GamesPage(BSMMApp app) {
			InitializeComponent();
			_viewModel = new GamesViewModel(app, async () => await Navigation.PopModalAsync());
			BindingContext = _viewModel;
		}

		private async void GamesListView_ItemTapped(object sender, ItemTappedEventArgs e) {
			if (e.Group is Gameset gameset) {
				_viewModel.Select(gameset.Id);
				await Navigation.PopModalAsync();
			}
		}
	}
}