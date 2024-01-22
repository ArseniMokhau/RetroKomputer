using EmulatorBackend.Services;
using Microsoft.AspNetCore.Builder;

namespace EmulatorBackend
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Other configurations...
            services.AddSingleton<EmulatorService>();
        }
    }
}
