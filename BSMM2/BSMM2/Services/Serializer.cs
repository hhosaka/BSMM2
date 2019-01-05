using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BSMM2.Services {

	public class Serializer<T> {

		private static readonly JsonSerializerSettings settings
			= new JsonSerializerSettings {
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Auto
			};

		public void Serialize(string filename, T obj) {
			using (var outf = new StreamWriter(filename)) {
				outf.Write(JsonConvert.SerializeObject(obj, settings));
			}
		}

		public T Deserialize(string filename) {
			using (var inf = new StreamReader(filename)) {
				return JsonConvert.DeserializeObject<T>(inf.ReadToEnd(), settings);
			}
		}
	}
}