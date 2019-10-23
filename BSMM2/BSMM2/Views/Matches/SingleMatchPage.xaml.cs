using BSMM2.Models;
using BSMM2.ViewModels.Matches;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static BSMM2.ViewModels.RoundViewModel;

namespace BSMM2.Views.Matches {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingleMatchPage : ContentPage {
		private SingleMatchViewModel _viewModel;

		public SingleMatchPage(Game game, MatchItem match) {
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