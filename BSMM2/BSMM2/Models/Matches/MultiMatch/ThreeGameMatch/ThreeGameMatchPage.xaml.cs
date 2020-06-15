using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Models.Matches.MultiMatch.ThreeGameMatch {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ThreeGameMatchPage : ContentPage {
		private ThreeGameMatchViewModel _viewModel;

		public ThreeGameMatchPage(ThreeGameMatchRule rule, Match match) {
			InitializeComponent();
			Title = String.Format("{0} Result", rule.Name);
			BindingContext = _viewModel = new ThreeGameMatchViewModel(rule, (MultiMatch)match, Back);
		}

		private void Back(object sender, EventArgs e)
			=> Back();

		private async void Back()
			=> await Navigation.PopModalAsync();
	}
}