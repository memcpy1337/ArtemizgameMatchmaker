using Microsoft.EntityFrameworkCore;

using Serilog;

namespace API;

/// <summary>
/// Entry Point Of Application
/// </summary>
public class Program
{
    /// <summary>
    /// Migrates pending database changes and starts the application
    /// </summary>
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("serilog.json")
            .Build();
            
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Application Starting...");
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            await host.RunAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "The Application failed to start...");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
        
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).UseSerilog().ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
}