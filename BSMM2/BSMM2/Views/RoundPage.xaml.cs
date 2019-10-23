using BSMM2.Models;
using BSMM2.ViewModels;
using BSMM2.Views.Matches;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static BSMM2.ViewModels.RoundViewModel;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoundPage : ContentPage {
		private RoundViewModel viewModel;

		public RoundPage(BSMMApp app) {
			InitializeComponent();

			BindingContext = viewModel = new RoundViewModel(app);
		}

		private async void OnMatchTapped(object sender, ItemTappedEventArgs args) {
			if (args.Item is MatchItem match && viewModel.IsPlaying)
				await Navigation.PushModalAsync(new NavigationPage(new SingleMatchPage(viewModel.Game, match)));

			RoundListView.SelectedItem = null;
		}
	}
}