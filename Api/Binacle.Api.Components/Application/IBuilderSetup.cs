using Microsoft.AspNetCore.Builder;

namespace Binacle.Api.Components.Application
{
    public interface IBuilderSetup : IWithSequenceOrder
    {
        void Execute(WebApplicationBuilder builder);
    }
}
