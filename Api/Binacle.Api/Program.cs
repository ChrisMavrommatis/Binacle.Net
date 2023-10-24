using Binacle.Api.Components.Application;
using Binacle.Api.Components.Helpers;
using System.Reflection;

namespace Binacle.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var builder = WebApplication.CreateBuilder(args);

            var builderSetupActions = ReflectionHelpers.GetInstancesOf<IBuilderSetup>(assemblies)
                .OrderBy(x => x.SequenceOrder)
                .ToList();

            foreach (var builderSetupAction in builderSetupActions)
                builderSetupAction.Execute(builder);

            var app = builder.Build();

            var appStartupActions = ReflectionHelpers.GetInstancesOf<IApplicationStartup>(assemblies)
                .OrderBy(x => x.SequenceOrder)
                .ToList();

            foreach (var appStartupAction in appStartupActions)
                appStartupAction.Execute(app);

            app.Run();
        }
    }
}