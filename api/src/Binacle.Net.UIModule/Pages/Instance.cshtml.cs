using System.Collections.Generic;
using Binacle.Net.UIModule.Models;
using Binacle.Net.UIModule.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Binacle.Net.UIModule.Pages;

internal class InstanceModel : AppletPageModel
{
	private readonly IOptions<FeatureOptions> featureOptions;

	public InstanceModel(
		AppletsService appletsService,
		IOptions<FeatureOptions> featureOptions,
		IWebHostEnvironment environment
	)
		: base(appletsService, "/Instance")
	{
		this.featureOptions = featureOptions;
		this.Environment = environment.EnvironmentName;
	}

	public string Version => Metadata.Version;

	public string Environment { get; }

	public IReadOnlyList<FeatureSwitch> Switches => FeatureSwitch.All;

	public bool IsOn(FeatureSwitch featureSwitch)
		=> this.featureOptions.Value.IsFeatureEnabled(featureSwitch.Feature);

	// Whoever switched it on recorded where it answers. The health path is configurable, so this is the only
	// way to link it correctly.
	public string? PathFor(FeatureSwitch featureSwitch)
		=> this.featureOptions.Value.PathFor(featureSwitch.Feature);

	public void OnGet()
	{
	}
}
