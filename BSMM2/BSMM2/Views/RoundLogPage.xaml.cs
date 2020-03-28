using BSMM2.Models;
using BSMM2.Resource;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoundLogPage : TabbedPage {

		public RoundLogPage(BSMMApp app) {
			InitializeComponent();
			Children.Add(CreatePage(new RoundPage(app), AppResources.TabTitleRound));

			Page CreatePage(Page page, string title) {
				var ret = new NavigationPage(page);
				ret.Title = title;
				return ret;
			}
		}
	}
}