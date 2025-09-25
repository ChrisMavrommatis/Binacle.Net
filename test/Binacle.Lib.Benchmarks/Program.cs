﻿using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks;

internal class Program
{
	static void Main(string[] args)
	{
		var config = ManualConfig.Create(DefaultConfig.Instance)
			.WithOptions(ConfigOptions.DisableLogFile);

		// custom order
		config.Orderer = new AttributeOrderer();

		BenchmarkSwitcher
			.FromAssembly(typeof(Program).Assembly)
			.Run(args, config);
	}
}
