using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMM2.Models {

	[JsonObject]
	public class Settings {

		[JsonProperty]
		public int Count { get; set; }

		[JsonProperty]
		public Rule Rule { get; set; }

		[JsonProperty]
		public string PlayerNamePrefix { get; set; }

		[JsonProperty]
		public IEnumerable<string> Entries { get; set; }
	}
}