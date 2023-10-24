namespace Binacle.Api.Components.Helpers
{
    public static class ReflectionHelpers
    {
        public static List<TInterface> GetInstancesOf<TInterface>(System.Reflection.Assembly[] asseblies)
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
                throw new InvalidOperationException($"Type of {nameof(TInterface)} must be an interface");

           return asseblies
                .SelectMany(x => x.GetTypes())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
                .Select(x => (TInterface)Activator.CreateInstance(x)!)
                .ToList();
        }
    }
}
