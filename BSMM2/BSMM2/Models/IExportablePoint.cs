namespace BSMM2.Models {

	public interface IExportablePoint : IPoint, IExportable {
		string Information { get; }

		int Value { get; }
	}
}