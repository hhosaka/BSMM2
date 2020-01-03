using System.Collections.Generic;

namespace BSMM2.Models {

	internal interface IGame {
		IEnumerable<Player> PlayersByOrder { get; }
	}
}