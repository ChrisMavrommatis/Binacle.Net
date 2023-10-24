using Microsoft.AspNetCore.Builder;

namespace Binacle.Api.Components.Application
{
    public interface IApplicationStartup : IWithSequenceOrder
    {
        void Execute(WebApplication app);
    }
}
