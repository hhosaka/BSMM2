using BSMM2.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace BSMM2Test {

	[TestClass]
	public class ExportTest {

		[TestMethod]
		public void ExportTest1() {
			var player = new Player("test");
			var buf = new StringBuilder();

			player.ExportTitle(new StringWriter(buf));
			Assert.AreEqual("name", buf.ToString());
		}
	}
}