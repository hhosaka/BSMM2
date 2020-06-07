using System.IO;

namespace BSMM2.Models {

	public interface IExportable {

		void ExportTitle(TextWriter writer);

		void ExportData(TextWriter writer);
	}
}