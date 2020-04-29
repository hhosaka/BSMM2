using BSMM2.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace BSMM2Test {

	[TestClass]
	public class ExportTest {

		[TestMethod]
		public void ExportTitleTest() {
			var player = new Player("test");
			var buf = new StringBuilder();

			player.ExportTitle(new StringWriter(buf));
			Assert.AreEqual("Name, Dropped, Point, WinPoint, LifePoint", buf.ToString());
		}

		[TestMethod]
		public void ExportDataTest() {
			var player = new Player("test");
			var buf = new StringBuilder();

			player.ExportData(new StringWriter(buf));
			Assert.AreEqual("\"test\", False, 0, 0, 0", buf.ToString());
		}
	}
}