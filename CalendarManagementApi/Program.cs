using Microsoft.EntityFrameworkCore;
using Serilog;
using CalendarManagementApi.Data;
using CalendarManagementApi.Services;
using CalendarManagementApi.Components;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/calendar-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Calendar Management API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddDbContext<CalendarDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<CalendarService>();
    builder.Services.AddScoped<IDateEventService, DateEventService>();
    builder.Services.AddScoped<IWaitingEventService, WaitingEventService>();
    builder.Services.AddScoped<IRepeatingEventService, RepeatingEventService>();

    builder.Services.AddControllers();
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAntiforgery();
    app.UseAuthorization();

    app.MapControllers();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
