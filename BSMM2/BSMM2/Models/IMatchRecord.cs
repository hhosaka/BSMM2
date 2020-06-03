namespace BSMM2.Models {

	public interface IMatchRecord {
		IPlayer Player { get; }
		IResult Result { get; }
		IPoint Point { get; }
	}
}