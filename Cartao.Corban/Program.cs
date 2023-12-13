using Cartao.Corban.Extensoes;
using Cartao.Corban.Interfaces;
using Cartao.Corban.Servicos;
using Polly;
using Polly.Extensions.Http;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//builder.Services.AddSingleton<HttpClient>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IBrokerConsumerService, BrokerConsumerService>();
builder.Services.AddScoped<IPropostaService, PropostaService>();

builder.Services.AddHttpClient<IPropostaService, PropostaService>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(2, retryAttempts => TimeSpan.FromSeconds(Math.Pow(2, retryAttempts)))
        );

builder.Services.ConfigureServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

//app.Run();
app.Configure();
var taskApi = app.RunAsync();
var gerenciadorHangFire = app.Services.GetService<IBrokerConsumerService>();
var taskHangFire = gerenciadorHangFire.ExecutaHngFire();
Task.WaitAll(taskApi, taskHangFire);