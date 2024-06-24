using System.Net;
using System.Reflection;
using System.Text.Json;
using Application;
using Application.Common.Interfaces;
using Application.Common.Models;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Netjection;

namespace API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }
        
    private const string ApiCorsPolicy = "APICorsPolicy";
    public void ConfigureServices(IServiceCollection services)
    {
        services.InjectServices(Assembly.GetAssembly(typeof(IRegisterUserService))!, Assembly.GetAssembly(typeof(Infrastructure.ServiceCollectionExtension))!, 
            Assembly.GetExecutingAssembly());

        services.AddCors(options => options.AddPolicy(ApiCorsPolicy, builder =>
            builder.AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(Configuration.GetValue<string[]>("Cors"))
                .AllowCredentials()
        ));

        services.AddInfrastructure(Configuration);
        services.AddApplication(Configuration);
        services.AddControllers().AddFluentValidation();

    }
        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (contextFeature != null)
                {
                    var result = JsonSerializer.Serialize(Response.Fail<object>(
                        $"{contextFeature.Error?.Message} {contextFeature.Error?.InnerException?.Message}"));
                    logger.LogError("Error occured {error} {@result}",contextFeature.Error, result);
                    await context.Response.WriteAsync(result);
                }
            });
        });

        app.UseRouting();
            
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}