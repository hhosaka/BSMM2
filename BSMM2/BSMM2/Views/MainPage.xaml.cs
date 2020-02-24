using BSMM2.Models;
using BSMM2.Resource;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage {

		public MainPage(BSMMApp app) {
			InitializeComponent();
			Children.Add(CreatePage(new PlayersPage(app), AppResources.TabTitlePlayer));
			Children.Add(CreatePage(new RoundPage(app), AppResources.TabTitleRound));
			//		Children.Add(CreatePage(new ItemsPage()));
			//		Children.Add(CreatePage(new AboutPage()));

			Page CreatePage(Page page, string title) {
				var ret = new NavigationPage(page);
				ret.Title = title;
				return ret;
			}
		}
	}
}