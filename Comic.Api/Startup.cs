using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Chloe;
using Chloe.MySql;
using Comic.Api.ReadModels;
using Comic.Cache;
using Comic.Cache.Interfaces;
using Comic.Common.BaseClasses;
using Comic.Common.Utilities;
using Comic.Repository;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using NSwag;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using static Comic.Common.BaseClasses.AppSettingsObject;

namespace Comic.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtObject>(Configuration.GetSection("Jwt"));
            services.AddControllers(o =>
            {
                if (this.HostEnvironment.EnvironmentName != "Development")
                    o.Filters.Add<AuthorizationFilter>();
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidIssuer = string.Empty,
                    ValidateAudience = false,
                    ValidAudience = string.Empty,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddSwaggerDocument(config =>
            {
                //config.OperationProcessors.Add(new SwaggerAuthFilter());
                config.DocumentName = "Comic";
                config.Version = "1.0.0";
                config.Title = "ComicAPI";
                config.Description = "Try it!";
            });

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(MappingConfig)));

            services.Scan(o => o.FromCallingAssembly().AddClasses().AsMatchingInterface().WithTransientLifetime());
            services.Scan(o => o.FromAssembliesOf(typeof(BaseRepository<>)).AddClasses().AsMatchingInterface().WithScopedLifetime());
            services.Scan(o => o.FromAssembliesOf(typeof(CacheProvider)).AddClasses(o => o.AssignableTo(typeof(ICache<>))).AsImplementedInterfaces().WithScopedLifetime());
            services.AddTransient<IDbContext, MySqlContext>(o => new MySqlContext(() => new MySqlConnection(Configuration["Connection:MySql"])));
            services.AddScoped<AuthorizationFilter>();
            services.AddLogging(o => o.ClearProviders());
            Log.Logger = CreateSerilog();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddSingleton<CacheProvider>();
            services.AddSingleton<CacheTokenProvider>();
            //services.AddSingleton<IConnectionMultiplexer>(o => ConnectionMultiplexer.Connect(Configuration["Connection:RedisToken"]));
            //services.AddStackExchangeRedisCache(o =>
            //{
            //    o.Configuration = Configuration["Connection:RedisToken"];
            //});
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                ExceptionHandler = async ctx =>
                {
                    var ex = ctx.Features.Get<IExceptionHandlerPathFeature>();
                    ctx.Response.StatusCode = 400;
                    ctx.Response.ContentType = "application/json";
                    var errorCode = int.TryParse(ex.Error.Message, out _) ? ex.Error.Message : "9999";
                    var errorDesc = int.TryParse(ex.Error.Message, out _) ? ErrorCodes.ErrorDesc[errorCode] : ex.Error.Message;
                    if (ex.Error.Message.IndexOf("out-of-order") > 1 || ex.Error.Message.IndexOf("cannot be called") > 1)
                    {
                        errorCode = "1234";
                        errorDesc = "Retry";
                    };
                    var result = JsonSerializer.Serialize(
                        ResponseUtility.CreateErrorResopnse(errorCode, errorDesc),
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    await ctx.Response.WriteAsync(result);
                }
            });

            app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            app.UseMiddleware<ResponseTimeMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hi!"); });
                endpoints.MapControllers();
            });
            app.UseSwaggerUi3();
            app.UseOpenApi(o => o.PostProcess = (doc, _) => doc.Schemes = new[] {
#if RELEASE
                OpenApiSchema.Https,
#endif
                OpenApiSchema.Http, });
        }

        private Serilog.ILogger CreateSerilog()
        {
            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .WriteTo.File($"{Directory.GetCurrentDirectory()}/Log/Log-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose);
            return config.CreateLogger();
        }
    }
}
