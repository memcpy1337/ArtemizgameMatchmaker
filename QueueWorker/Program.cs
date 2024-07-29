using System.Net;
using System.Reflection;
using System.Text.Json;
using Application;
using Application.Common.Models;
using Application.Services;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Netjection;
using Microsoft.Extensions.Configuration.UserSecrets;

using QueueWorker;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var Scope = host.Services.CreateScope())
        {
            var context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        //// seed some queue messages
        //var queueSender = (IQueueSender)host.Services.GetRequiredService(typeof(IQueueSender));
        //for (int i = 0; i < 10; i++)
        //{
        //    queueSender.SendMessageToQueue("https://google.com", "urlcheck");
        //}

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {

                services.AddSingleton<IQueueWorkerService, QueueWorkerService>();

                services.AddInfrastructure(hostContext.Configuration);
                services.AddApplication(hostContext.Configuration);
                // services.AddControllers().AddFluentValidation();

                var workerSettings = new WorkerSettings();
                hostContext.Configuration.Bind(nameof(WorkerSettings), workerSettings);
                services.AddSingleton(workerSettings);

                services.AddHostedService<Worker>();
            });
}