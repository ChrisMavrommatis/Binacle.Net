using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Binacle.Net.Api.UIModule.ExtensionMethods;

internal static class EnumExtensions
{
	public static Dictionary<T, string> ToKeyValue<T>() where T : Enum
	{
		return Enum.GetValues(typeof(T))
			.Cast<T>()
			.ToDictionary(
				e => e,
				e => e.GetDisplayName()
			);
	}
	
	 private static string GetDisplayName(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var displayAttribute = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>()
                .FirstOrDefault();
            return displayAttribute?.Name ?? value.ToString();
        }
}
