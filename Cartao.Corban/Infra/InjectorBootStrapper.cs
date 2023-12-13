using Cartao.Corban.Interfaces;
using Cartao.Corban.Servicos;
using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace Cartao.Corban.Infra
{
    public static class InjectorBootStrapper
    {
        public static void RegistreServices(WebApplicationBuilder builder)
        {
            builder.Services.Configuration();
        }

        private static void Configuration(this IServiceCollection services) 
        {
            services.AddTransient<IBrokerConsumerService, BrokerConsumerService>();
            services.AddScoped<IPropostaService, PropostaService>();

            services.AddHttpClient<IPropostaService, PropostaService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(
                    HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                        .WaitAndRetryAsync(2, retryAttempts => TimeSpan.FromSeconds(Math.Pow(2, retryAttempts)))
                    );
        }
    }
}
