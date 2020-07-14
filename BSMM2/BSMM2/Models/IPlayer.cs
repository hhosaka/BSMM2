namespace BSMM2.Models {

	public interface IPlayer : IExportable {
		string Name { get; }

		IExportablePoint Point { get; }
	}
}