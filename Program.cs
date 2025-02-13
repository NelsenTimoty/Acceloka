using AccelokaAPI.Data;
using AccelokaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// ✅ Configure Serilog logging
var logFilePath = Path.Combine("logs", $"Log-{DateTime.UtcNow:yyyyMMdd}.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()  // ✅ Log level: Information and above
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .WriteTo.Console() // Optional: Log to console
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day) // ✅ Save logs to /logs folder
    .CreateLogger();


// ✅ Add services to the container
builder.Services.AddControllers();

// ✅ Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Configure Database Connection (use appsettings.json connection string)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Register Repositories
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IBookedTicketRepository, BookedTicketRepository>();

// ✅ Configure RFC 7807 Problem Details for Validation Errors
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();

var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}

var app = builder.Build();

// ✅ Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
