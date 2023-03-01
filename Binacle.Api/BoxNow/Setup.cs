using Binacle.Api.BoxNow.Models;

namespace Binacle.Api.BoxNow
{
    public static class Setup
    {
        public static void AddBoxNowConfiguration(this WebApplicationBuilder builder)
        {
            var boxNowConfigRoot = builder.Configuration.AddJsonFile("Configurations/BoxNow.json", optional: false, reloadOnChange: true).Build();
            builder.Services.Configure<BoxNowConfiguration>(boxNowConfigRoot.GetSection("BoxNowConfiguration"));
        }
    }
}
