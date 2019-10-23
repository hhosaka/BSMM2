using BSMM2.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage {

		public MainPage(BSMMApp app) {
			InitializeComponent();
			Children.Add(CreatePage(new PlayersPage(app)));
			Children.Add(CreatePage(new RoundPage(app)));
			Children.Add(CreatePage(new ItemsPage()));
			Children.Add(CreatePage(new AboutPage()));

			Page CreatePage(Page page) {
				var ret = new NavigationPage(page);
				ret.Title = page.Title;
				return ret;
			}
		}
	}
}