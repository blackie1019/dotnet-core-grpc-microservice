#region

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSite.Common.Data.Utilities;
using MockSite.Core.Factories;
using MockSite.Core.Interfaces;
using MockSite.Core.Repositories;
using MockSite.DomainService.Utilities;
using MockSite.Message;
using OpenTracing.Contrib.Grpc.Interceptors;

#endregion

namespace MockSite.DomainService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton<UserService.UserServiceBase, UserServiceImpl>();
            services.AddSingleton<IUserService, Core.Services.UserService>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IMongoUserRepository, MongoUserRepository>();
            services.AddSingleton<IRedisUserRepository, RedisUserRepository>();
            services.AddSingleton<RedisConnectHelper>();
            services.AddSingleton<CurrencyService.CurrencyServiceBase, CurrencyServiceImpl>();
            services.AddSingleton<LocalizationService.LocalizationServiceBase, LocalizationServiceImpl>();
            services.AddSingleton<ICurrencyService, Core.Services.CurrencyService>();
            services.AddSingleton<ILocalizationService, Core.Services.LocalizationService>();
            services.AddSingleton<ICurrencyRepository, CurrencyRepository>();
            services.AddSingleton<ILocalizationRepository, LocalizationRepository>();
            services.AddSingleton<UserRepositoryFactory>();
            services.AddSingleton<SqlConnectionHelper>();

            const string serviceName = "MockSite.DomainService";
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var tracer = TracingHelper.InitTracer(serviceName, loggerFactory);

            services.AddGrpc(options => options.Interceptors.Add<ServerTracingInterceptor>(tracer));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<UserServiceImpl>();
                endpoints.MapGrpcService<CurrencyServiceImpl>();
                endpoints.MapGrpcService<LocalizationServiceImpl>();
            });
        }
    }
}