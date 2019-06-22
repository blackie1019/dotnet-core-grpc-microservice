#region

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Web.Constants;
using MockSite.Web.Enums;
using MockSite.Web.Models;
using MockSite.Web.Services;
using MockSite.Web.Services.Implements;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Contrib.Grpc.Interceptors;
using OpenTracing.Util;
using Swashbuckle.AspNetCore.Swagger;

#endregion

namespace MockSite.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 加入 OpenTracing
            services.AddOpenTracing().AddSingleton<ITracer>(serviceProvider =>
            {
                const string serviceName = "MockSite.Web";
                var tracer = new Tracer.Builder(serviceName)
                    .WithSampler(new ConstSampler(true))
                    .Build();

                // 註冊 Jaeger tracer
                GlobalTracer.Register(tracer);
                return tracer;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "MockSite API", Version = "v1"});
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Enumerable.Empty<string>()}
                });
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

            // Configure JWT authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = AppSettingsHelper.Instance.GetValueFromKey(AppSetting.JwtIssuerKey),
                        ValidAudience = AppSettingsHelper.Instance.GetValueFromKey(AppSetting.JwtAudienceKey),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(AppSettingsHelper.Instance.GetValueFromKey(AppSetting.JwtSecretKey))
                        )
                    };
                });

            // Configure DI for application services
            services.AddScoped<IUserService, UserService>();

            // Register gRPC Service
            var userHost =
                $"{ConsulSettingHelper.Instance.GetValueFromKey(HostNameConst.TestKey)}:{ConsulSettingHelper.Instance.GetValueFromKey(PortConst.TestKey)}";
            var tracingInterceptor = new ClientTracingInterceptor(GlobalTracer.Instance);

            services.AddSingleton(new Message.UserService.UserServiceClient(
                CreateKeepAliveWithoutCallChannel(userHost).Intercept(tracingInterceptor))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = async context =>
                {
                    if (Regex.IsMatch(context.Request.Path.Value, "^/api/", RegexOptions.IgnoreCase))
                    {
                        await HandleApiException(context);
                    }
                    else if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseExceptionHandler("/Error");
                        app.UseHsts();
                    }
                }
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseSwagger().UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseAuthentication();
            app.UseMvc(routes => { routes.MapRoute("default", "{controller}/{action=Index}/{id?}"); });
            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "ClientApp";
                    spa.UseReactDevelopmentServer("start");
                }
                else if (env.IsProduction())
                {
                    spa.Options.SourcePath = "ClientApp/build";
                }
            });
        }

        private Channel CreateKeepAliveWithoutCallChannel(string hostString)
        {
            return new Channel(hostString, ChannelCredentials.Insecure, new List<ChannelOption>()
            {
                new ChannelOption("grpc.keepalive_permit_without_calls", 1),
                new ChannelOption("grpc.http2.max_pings_without_data", 0),
                new ChannelOption("grpc.keepalive_time_ms", 1000 * 60 * 20)
            });
        }

        private async Task HandleApiException(HttpContext context)
        {
            var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
            var response = new ResponseBaseModel<string>(ResponseCode.GeneralError, null, exception.StackTrace);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) HttpStatusCode.OK;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response), Encoding.UTF8);
        }
    }
}