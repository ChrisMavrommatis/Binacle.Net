using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using ChrisMavrommatis.Results.Typed;
using ChrisMavrommatis.Results.Unions;

namespace Binacle.Net.Api.ServiceModule.Domain.Users.Data;

public interface IUserRepository
{
	Task<OneOf<User, Error>> GetAsync(string email, CancellationToken cancellationToken = default);
	Task<OneOf<Ok, Error>> CreateAsync(User user, CancellationToken cancellationToken = default);
	Task<OneOf<Ok, Error>> DeleteAsync(string email, CancellationToken cancellationToken = default);
	Task<OneOf<Ok, Error>> UpdateAsync(User user, CancellationToken cancellationToken = default);
}
