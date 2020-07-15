using System.Collections.Generic;

namespace BSMM2.Models {

	public interface IExportable {

		IDictionary<string, string> Export(IDictionary<string, string> data);
	}
}