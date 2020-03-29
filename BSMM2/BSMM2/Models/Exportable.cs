using System.IO;

namespace BSMM2.Models {

	public interface Exportable {

		void ExportTitle(TextWriter writer);

		void Export(TextWriter writer);
	}
}