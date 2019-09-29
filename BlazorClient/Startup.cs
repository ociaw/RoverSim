using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RoverSim.BlazorClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(CreateAiProvider);
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }

        private AiProvider CreateAiProvider(System.IServiceProvider services)
        {
            return new AiProvider
            {
                new Ais.FixedStateAiFactory(),

                new ScratchAis.RandomAiFactory(),
                new ScratchAis.IntelligentRandomAiFactory(),
                new ScratchAis.MarkIFactory(),
                new ScratchAis.MarkIIFactory()
            };
        }
    }
}
