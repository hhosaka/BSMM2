using System.Collections.Generic;

namespace BSMM2.Models {

	public interface IMatch {
		IEnumerable<IMatchRecord> Records { get; }
		IMatchRecord Record1 { get; }
		IMatchRecord Record2 { get; }

		void SetResults((IResult player1, IResult player2) points);
	}
}