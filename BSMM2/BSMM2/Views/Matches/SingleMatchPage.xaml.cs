using BSMM2.Models;
using BSMM2.Models.Matches;
using BSMM2.ViewModels.Matches;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views.Matches {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingleMatchPage : ContentPage {
		private SingleMatchViewModel _viewModel;

		public SingleMatchPage(Rule rule, Match match) {
			InitializeComponent();
			BindingContext = _viewModel = new SingleMatchViewModel(rule as SingleMatchRule, match);
		}

		private async void Create_Clicked(object sender, EventArgs e) {
			_viewModel.Execute();
			MessagingCenter.Send(this, "SetMatch");
			await Navigation.PopModalAsync();
		}
	}
}