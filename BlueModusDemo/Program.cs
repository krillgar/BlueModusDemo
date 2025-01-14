using BlueModusDemo.Middleware;
using BlueModusDemo.Services.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddLogging()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .UseServices()
    .UseCustomRedirection(builder.Configuration);

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

var tokenSource = new CancellationTokenSource();

RedirectMiddleware.ConfigureRoutes(app.Services, tokenSource.Token);

app.Run();
