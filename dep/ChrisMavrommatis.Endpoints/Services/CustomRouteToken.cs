using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace ChrisMavrommatis.Endpoints.Services;

internal class CustomRouteToken : IApplicationModelConvention
{
	private readonly string _tokenRegex;
	private readonly Func<ControllerModel, string?> _valueGenerator;

	public CustomRouteToken(string tokenName, Func<ControllerModel, string?> valueGenerator)
	{
		_tokenRegex = $@"(\[{tokenName}])(?<!\[\1(?=]))";
		_valueGenerator = valueGenerator;
	}

	public void Apply(ApplicationModel application)
	{
		foreach (var controller in application.Controllers)
		{
			string? tokenValue = _valueGenerator(controller);
			UpdateSelectors(controller.Selectors, tokenValue);
			UpdateSelectors(controller.Actions.SelectMany(a => a.Selectors), tokenValue);
		}
	}

	private void UpdateSelectors(IEnumerable<SelectorModel> selectors, string? tokenValue)
	{
		foreach (var selector in selectors.Where(s => s.AttributeRouteModel != null))
		{
			selector.AttributeRouteModel.Template = InsertTokenValue(selector.AttributeRouteModel.Template, tokenValue);
			selector.AttributeRouteModel.Name = InsertTokenValue(selector.AttributeRouteModel.Name, tokenValue);
		}
	}

	private string? InsertTokenValue(string? template, string? tokenValue)
	{
		if (template is null)
		{
			return template;
		}

		return Regex.Replace(template, _tokenRegex, tokenValue);
	}
}
