using Application.Common.Interfaces;
using Application.Common.Validators.Match;
using FluentValidation;
using Infrastructure.Common.Models;
using Infrastructure.Consumers;
using Infrastructure.Persistance;
using Infrastructure.Repositories;
using Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace Infrastructure;

/// <summary>
/// Extension Class For <see cref="IServiceCollection"/> Interface
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Injects Infrastructure Dependencies Into Dependency Injection Container
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> Interface</param>
    /// <param name="configuration"><see cref="IConfiguration"/> Interface</param>
    public static void AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IQueueService, QueueService>();
        services.AddTransient<IMatchRepository, MatchRepository>();
        services.AddTransient<IPublishService, PublishService>();

        services.AddValidatorsFromAssemblyContaining<UserQueueDtoValidator>();

        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter(); //user-created-event
            busConfig.AddConsumer<AddUserToQueueConsumer>();
#if DEBUG
            var stringSettings = Environment.GetEnvironmentVariable("MessageBrokerDebug");
            var settings = JsonConvert.DeserializeObject<MessageBrokerSettings>(stringSettings);
#else
            var stringSettings = Environment.GetEnvironmentVariable("MessageBroker");
            var settings = JsonConvert.DeserializeObject<MessageBrokerSettings>(stringSettings);
#endif
            busConfig.UsingRabbitMq((context, configuration) =>
            {
                configuration.Host(new Uri(settings.Host!), h =>
                {
                    h.Username(settings.Username!);
                    h.Password(settings.Password!);
                });

                configuration.ConfigureEndpoints(context);
            });
        });


#if DEBUG
        var redisStringSettings = Environment.GetEnvironmentVariable("RedisSettingsMatchmakerDebug");
        var redisSettings = JsonConvert.DeserializeObject<RedisSettings>(redisStringSettings);
#else
        var redisStringSettings = Environment.GetEnvironmentVariable("RedisSettings");
        var redisSettings = JsonConvert.DeserializeObject<RedisSettings>(redisStringSettings);
#endif
        var config = new ConfigurationOptions
        {
            EndPoints = { redisSettings.Host },
            User = redisSettings.User,
            Password = redisSettings.Password,
            AbortOnConnectFail = false
        };

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(config));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            string connString = string.Empty;
#if DEBUG
            connString = Environment.GetEnvironmentVariable("CONNSTRINGMATCHMAKINGDEBUG");
#else
                connString = Environment.GetEnvironmentVariable("CONNSTRING");
#endif
            options.UseNpgsql(connString,
                builder =>
                {
                    builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    //EF allows you to specify that a given LINQ query should be split into multiple SQL queries.
                    //Instead of JOINs, split queries generate an additional SQL query for each included collection navigation
                    //More about that: https://docs.microsoft.com/en-us/ef/core/querying/single-split-queries
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);


        services.AddTransient<IApplicationDbContext>(x => x.GetService<ApplicationDbContext>()!);
    }
}