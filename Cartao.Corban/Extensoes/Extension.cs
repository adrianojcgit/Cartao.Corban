using Hangfire;
using Hangfire.MemoryStorage;

namespace Cartao.Corban.Extensoes
{
    public static class Extension
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddHangfire(op =>
            {
                op.UseMemoryStorage()
                .UseRecommendedSerializerSettings();
            });
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
            {
                Attempts = 1,
                DelaysInSeconds = new int[] { 300 }
            });
            services.AddHangfireServer();
        }

        public static void Configure(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
        }
    }
}
