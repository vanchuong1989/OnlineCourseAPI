
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OnlineCourse.API.Middlewares;
using OnlineCourse.Data;
using OnlineCourse.Data.Entities;
using OnlineCourse.Service;
using Serilog;
using Serilog.Templates;
using System.Net;

namespace OnlineCourse.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Configure Serilog with the settings
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console ()
                .WriteTo.Debug()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .CreateBootstrapLogger();
            try
            {
                //we have 2 parts here , one is service configuration for DI and second one is Middlewares

                #region Service Configuration
                var builder = WebApplication.CreateBuilder(args);
                var configuration = builder.Configuration;


                builder.Services.AddApplicationInsightsTelemetry();

                builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.Console(new ExpressionTemplate(
                    // Include trace and span ids when present.
                    "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}"))
                .WriteTo.ApplicationInsights(
                  services.GetRequiredService<TelemetryConfiguration>(),
                  TelemetryConverter.Traces));

                Log.Information("Starting the SmartLearnByKarthik API...");



                //DB configuration goes here

                //Tips, if we want to see what are paramters we can enable here but it will show alls sensitive data
                //so only used for dev purpose should no go to PRODUCTION!
                builder.Services.AddDbContextPool<OnlineCourseDbContext>(option =>
                {
                    option.UseSqlServer(
                        configuration.GetConnectionString("DbContext"),
                        providerOptions => providerOptions.EnableRetryOnFailure()
                    );
                    //option.EnableSensitiveDataLogging();
                });


                //In production, modify this with the actual domains you want to allow
                builder.Services.AddCors(o=>o.AddPolicy("default", builder =>
                {
                    builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                }));


                // Add services to the container.

                builder.Services.AddControllers();
               

                //configure service DI here
                builder.Services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
                builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();
                builder.Services.AddScoped<ICourseRepository, CourseRepository>();
                builder.Services.AddScoped<ICourseService, CourseService>();
               
                builder.Services.AddTransient<RequestBodyLoggingMiddleware>();
                builder.Services.AddTransient<ResponseBodyLoggingMiddleware>();

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                #endregion

                #region Middleware
                var app = builder.Build();


                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        Log.Error(exception, "Unhandled exception occurred. {ExceptionDetails}", exception?.ToString());
                        Console.WriteLine(exception?.ToString());
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
                    });
                });

                

              
                app.UseCors("default");

                // Configure the HTTP request pipeline.
                //if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseMiddleware<RequestResponseLoggingMiddleware>();
                //Enable our custom middleware
                app.UseMiddleware<RequestBodyLoggingMiddleware>();
                app.UseMiddleware<ResponseBodyLoggingMiddleware>();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();

                #endregion
            }
            catch (Exception ex)
            {

                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush ();
            }
                

        }
    }
}
