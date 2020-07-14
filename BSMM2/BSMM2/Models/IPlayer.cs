namespace BSMM2.Models {

	public interface IPlayer : IExportable {
		string Name { get; }

		bool Dropped { get; }

		IExportablePoint Point { get; }

		IExportablePoint OpponentPoint { get; }

		int ByeMatchCount { get; }

		bool HasGapMatch { get; }

		void Commit(Match match);
	}
}