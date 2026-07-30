using FluentValidation;

namespace Binacle.Net;

public static class RuleBuilderValidationExtensions
{
	public static IRuleBuilderOptions<T, TProperty> MustNotThrow<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Action<TProperty> action) 
	{
		return ruleBuilder.Must((x, val) =>
		{
			try
			{
				action(val);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		});
	}
	
	public static IRuleBuilderOptions<T, TProperty> MustNotThrow<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Action<T, TProperty> action) 
	{
		return ruleBuilder.Must((x, val) =>
		{
			try
			{
				action(x, val);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		});
	}
}
