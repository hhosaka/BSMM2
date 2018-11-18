using System;
using System.Collections.Generic;
using System.Text;

namespace BSMM2.Models {

	public interface IPoint : IComparable<IPoint> {
		int MatchPoint { get; }
	}
}