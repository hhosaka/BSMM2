namespace BSMM2.Models {

	public interface IMatch {
		IMatchRecord Record1 { get; }
		IMatchRecord Record2 { get; }

		void SetResults((IResult player1, IResult player2) points);
	}
}