using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Models.Matches.SingleMatch {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingleMatchLPPage : ContentPage {
		private SingleMatchLPViewModel _viewModel;

		public SingleMatchLPPage(SingleMatchRule rule, Match match) {
			InitializeComponent();
			Title = String.Format("{0} Result", rule.Name);
			BindingContext = _viewModel = new SingleMatchLPViewModel(rule, match, Back);
		}

		private void Back(object sender, EventArgs e)
			=> Back();

		private async void Back()
			=> await Navigation.PopModalAsync();
	}
}