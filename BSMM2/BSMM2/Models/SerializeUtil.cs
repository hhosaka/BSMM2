using BSMM2.Services;
using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace BSMM2.Models {

	public class SerializeUtil {
		private IsolatedStorageFile _store;

		public T Load<T>(string filename, Func<T> initiator) where T : new() {
			if (_store.FileExists(filename)) {
				using (var strm = _store.OpenFile(filename, FileMode.Open))
				using (var reader = new StreamReader(strm)) {
					return new Serializer<T>().Deserialize(reader);
				}
			}
			return initiator();
		}

		public void Save<T>(T data, string filename) {
			using (var strm = _store.CreateFile(filename))
			using (var writer = new StreamWriter(strm)) {
				new Serializer<T>().Serialize(writer, data);
			}
		}

		public void Delete(string filename) {
			_store.DeleteFile(filename);
		}

		public SerializeUtil() {
			_store = IsolatedStorageFile.GetUserStoreForApplication();
		}
	}
}