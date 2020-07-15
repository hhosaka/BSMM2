using System.Collections.Generic;
using System.IO;

namespace BSMM2.Models {

	public interface IExportable {

		void ExportTitle(TextWriter writer, string prefix = "");

		void ExportData(TextWriter writer);

		IDictionary<string, string> Export(IDictionary<string, string> data);
	}
}