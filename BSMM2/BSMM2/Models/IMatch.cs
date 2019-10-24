using System.Collections.Generic;

namespace BSMM2.Models {

	public interface IMatch {
		IEnumerable<IMatchRecord> Records { get; }

		void SetResults((IResult player1, IResult player2) points);
	}
}