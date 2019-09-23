using Xamarin.Forms;

namespace BSMM2.Views {

	public class NewGamePage : ContentPage {

		public NewGamePage() {
			Content = new StackLayout {
				Children = {
					new Label { Text = "New Game Page" }
				}
			};
		}
	}
}