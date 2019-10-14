using BSMM2.Models;
using BSMM2.Models.Rules.Match;
using BSMM2.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MatchPage : ContentPage {
		private MatchViewModel _viewModel;

		public MatchPage(Rule rule, Match match) {
			InitializeComponent();
			BindingContext = _viewModel = new MatchViewModel(rule as SingleMatchRule, match);
		}

		private async void Create_Clicked(object sender, EventArgs e) {
			_viewModel.Execute();
			MessagingCenter.Send(this, "SetMatch");
			await Navigation.PopModalAsync();
		}
	}
}