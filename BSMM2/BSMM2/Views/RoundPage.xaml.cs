using BSMM2.Models;
using BSMM2.ViewModels;
using System.Threading.Tasks;
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
			viewModel.OnMatchingFailed += OnMatchingFailed;
		}

		private async Task OnMatchingFailed() {
			await DisplayAlert("Alert", "Fail to generate match", "OK");//TODO : guide to match settings
		}

		private async void OnMatchTapped(object sender, ItemTappedEventArgs args) {
			if (args.Item is MatchItem match && viewModel.IsPlaying)
				await Navigation.PushModalAsync(new NavigationPage(viewModel.Game.CreateMatchPage(match)));

			RoundListView.SelectedItem = null;
		}
	}
}