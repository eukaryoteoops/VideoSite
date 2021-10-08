using System;
using System.Diagnostics.CodeAnalysis;
using Chloe;
using Chloe.MySql;
using Comic.Repository;
using Comic.Schedule.Jobs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Slack.Webhooks;

namespace Comic.Schedule
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
            services.AddMemoryCache();
            services.Scan(o => o.FromAssembliesOf(typeof(BaseRepository<>)).AddClasses().AsMatchingInterface().WithTransientLifetime());

            services.AddTransient<IDbContext, MySqlContext>(o => new MySqlContext(() => new MySqlConnection(Configuration["Connection:MySql"])));
            services.AddTransient<SlackClient>(o => new SlackClient("https://hooks.slack.com/services/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"));
            services.AddHangfire(o =>
            {
                o.UseRecommendedSerializerSettings();
                o.UseMemoryStorage();
                o.UseConsole();
            });
            services.AddHangfireServer(o =>
            {
                o.SchedulePollingInterval = TimeSpan.FromSeconds(15);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHangfireDashboard(new DashboardOptions
                {
                    IsReadOnlyFunc = (context) => true,
                    Authorization = new[] { new DashboardAuthorizeFilter() }
                });
            });

            //BackgroundJob.Enqueue<TestJob>(o => o.Trigger(null));

            //同步影片
            RecurringJob.AddOrUpdate<VideoSyncJob>(o => o.Trigger(null), Cron.Daily(10));
            //轉單給11003
            RecurringJob.AddOrUpdate<UpdateOrderJob>(o => o.Trigger(null), "0 16 * * *");
            //設定限時免費
            RecurringJob.AddOrUpdate<UpdateFreeComics>(o => o.Trigger(null), "50 9 * * *");
            //更新每日影片
            RecurringJob.AddOrUpdate<UpdateDailyComics>(o => o.Trigger(null), "50 9 * * *");
            //統計觀看收藏數
            RecurringJob.AddOrUpdate<VideoStatisticJob>(o => o.Trigger(null), Cron.MinuteInterval(10));
            RecurringJob.AddOrUpdate<ComicStatisticJob>(o => o.Trigger(null), Cron.MinuteInterval(10));
        }
    }
    public class DashboardAuthorizeFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
