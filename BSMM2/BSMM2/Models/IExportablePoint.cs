﻿namespace BSMM2.Models {

	public interface IExportablePoint : IPoint, IExportable {
		string Information { get; }
	}
}