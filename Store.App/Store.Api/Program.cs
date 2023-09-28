using Microsoft.EntityFrameworkCore;
using Serilog;
using Store.Api.Models;
using Store.Api.Repositories;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    //.WriteTo.File("logs/store_.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Database setup and connection
builder.Services.AddDbContext<StoreContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
