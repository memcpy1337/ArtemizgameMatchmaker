using Application.Common.Behaviours;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Services;
using FluentValidation;
using Forbids;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Application;

/// <summary>
/// Extension Class For <see cref="IServiceCollection"/> Interface
/// </summary>
public static class ServiceCollectionExtension 
{
    /// <summary>
    /// Injects Application Dependencies Into Dependency Injection Container
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> Interface</param>
    /// <param name="configuration"><see cref="IConfiguration"/> Interface</param>
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddTransient<IQueueWorkerService, QueueWorkerService>();
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IQueueService, QueueService>();
        services.AddTransient<ITaskWorkerService, TaskWorkerService>();
        services.AddTransient<ITaskService, TaskService>();

        services.AddValidatorsFromAssemblyContaining(typeof(Application.AssemblyReference), includeInternalTypes: true);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(TaskCanceledExceptionBehaviour<,>));
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TaskCanceledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddHttpContextAccessor();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizedUserContextBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UserIpContextBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizedServerContextBehaviour<,>));

        services.AddHostedService<QueueTick>();
        services.AddHostedService<TaskQueueTick>();
        services.AddForbids();
    }
}