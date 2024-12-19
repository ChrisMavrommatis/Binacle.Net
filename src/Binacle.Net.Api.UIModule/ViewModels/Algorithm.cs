using System.ComponentModel.DataAnnotations;

namespace Binacle.Net.Api.UIModule.ViewModels;

internal enum Algorithm
{
	[Display(Name = "First Fit Decreasing")]
	FirstFitDecreasing,
	[Display(Name = "Worst Fit Decreasing")]
	WorstFitDecreasing,
	[Display(Name = "Best Fit Decreasing")]
	BestFitDecreasing
}
