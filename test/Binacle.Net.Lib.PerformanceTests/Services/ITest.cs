﻿using Binacle.Net.Lib.PerformanceTests.Models;

namespace Binacle.Net.Lib.PerformanceTests.Services;

internal interface ITest
{
	TestResultList Run();
}
