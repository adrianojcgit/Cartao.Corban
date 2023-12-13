using Cartao.Corban.Extensoes;
using Cartao.Corban.Infra;
using Cartao.Corban.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureServices();
InjectorBootStrapper.RegistreServices(builder);

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