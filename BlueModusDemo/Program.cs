using BlueModusDemo.Middleware;
using BlueModusDemo.Services.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .UseServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RedirectMiddleware>();

app.MapControllers();

app.Run();
