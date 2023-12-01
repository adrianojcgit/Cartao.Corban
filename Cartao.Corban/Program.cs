using Cartao.Corban.Extensoes;
using Cartao.Corban.Interfaces;
using Cartao.Corban.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IBrokerConsumerService, BrokerConsumerService>();

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