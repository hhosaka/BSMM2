using BSMM2.Resource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BSMM2.Models {

	[JsonObject]
	internal class BYE : IPlayer {

		[JsonIgnore]
		public string Name => AppResources.TextBYE;

		public IDictionary<string, string> Export(IDictionary<string, string> data) {
			throw new NotImplementedException();
		}

		public void ExportData(TextWriter writer) => throw new NotImplementedException();

		public void ExportTitle(TextWriter writer, string index) => throw new NotImplementedException();
	}
}