using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Models.Matches.SingleMatch {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingleMatchPage : ContentPage {
		private SingleMatchViewModel _viewModel;

		public SingleMatchPage(Game game, IMatch match) {
			InitializeComponent();
			Title = "Single Match Result";
			BindingContext = _viewModel = new SingleMatchViewModel(game, match);
		}

		private async void OnResultTapped(object sender, SelectedItemChangedEventArgs args) {
			if (_viewModel.Update())
				await Navigation.PopModalAsync();
		}
	}
}