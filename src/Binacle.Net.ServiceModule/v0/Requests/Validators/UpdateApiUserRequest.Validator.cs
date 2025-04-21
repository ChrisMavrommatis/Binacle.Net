// using Binacle.Net.ServiceModule.Domain.Validators;
// using FluentValidation;
//
// namespace Binacle.Net.ServiceModule.v0.Requests.Validators;
//
// internal class UpdateApiUserRequestValidator : AbstractValidator<UpdateApiUserRequestWithBody>
// {
// 	public UpdateApiUserRequestValidator()
// 	{
// 		Include(x => new EmailValidator());
// 		RuleFor(x => x.Body!).SetValidator(x => new UpdateApiUserRequestBodyValidator());
// 	}
// }
//
// internal class UpdateApiUserRequestBodyValidator : AbstractValidator<UpdateApiUserRequest>
// {
// 	public UpdateApiUserRequestBodyValidator()
// 	{
// 		When(x => !x.Status.HasValue, () =>
// 		{
// 			RuleFor(y => y.Type).Must(y => y.HasValue).WithMessage("'Type' must not be empty when 'Status' is empty and only accepts the following values: 'User', 'Admin'");
// 		});
//
// 		When(x => !x.Type.HasValue, () =>
// 		{
// 			RuleFor(y => y.Status).Must(y => y.HasValue).WithMessage("'Status' must not be empty when 'Type' is empty and only accepts the following values: 'Active', 'Inactive'");
// 		});
// 	}
// }
