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

		public void Serialize(TextWriter w, T obj) {
			w.Write(JsonConvert.SerializeObject(obj, settings));
		}

		public void Serialize(string filename, T obj) {
			using (var w = new StreamWriter(filename)) {
				Serialize(w, obj);
			}
		}

		public T Deserialize(TextReader r) {
			return JsonConvert.DeserializeObject<T>(r.ReadToEnd(), settings);
		}

		public T Deserialize(string filename) {
			using (var r = new StreamReader(filename)) {
				return Deserialize(r);
			}
		}
	}
}