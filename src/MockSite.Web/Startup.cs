#region

using System.Collections.Generic;
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
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Web.Constants;
using MockSite.Web.Services;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Contrib.Grpc.Interceptors;
using OpenTracing.Util;
using MockSite.Common.Core.Models;
using MockSite.Message;
using ResponseCode = MockSite.Common.Core.Enums.ResponseCode;
using UserService = MockSite.Web.Services.Implements.UserService;

#endregion

namespace MockSite.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private const string CorsPolicy = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
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

            services.AddMvc().AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "MockSite API", Version = "v1"});
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>(0)
                    }
                });
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(config => { config.RootPath = "ClientApp/build"; });

            // Configure JWT authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration.GetSection(AppSetting.JwtIssuerKey).Value,
                        ValidAudience = _configuration.GetSection(AppSetting.JwtAudienceKey).Value,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_configuration.GetSection(AppSetting.JwtSecretKey).Value)
                        )
                    };
                });

            // Configure DI for application services
            services.AddScoped<IUserService, UserService>();

            var consulIp = _configuration.GetSection(ConsulConfigConst.ConsulIp).Value;
            var consulPort = _configuration.GetSection(ConsulConfigConst.ConsulPort).Value;
            var consulModule = _configuration.GetSection(ConsulConfigConst.ConsulModule).Value;

            var consulProvider =
                ConsulConfigProvider.LoadConsulConfig($"http://{consulIp}:{consulPort}/v1/kv/",
                    consulModule.Split(','));
            var consul = new ConfigurationBuilder()
                .AddJsonFile(consulProvider,"test.json",true,false)
                .Build();
            
            // Register gRPC Service
            var userHost =
                $"{consul[HostNameConst.TestKey]}:{consul[PortConst.TestKey]}";
            var tracingInterceptor = new ClientTracingInterceptor(GlobalTracer.Instance);

            services.AddSingleton(new Message.UserService.UserServiceClient(
                CreateKeepAliveWithoutCallChannel(userHost).Intercept(tracingInterceptor))
            );

            services.AddSingleton(new CurrencyService.CurrencyServiceClient(
                CreateKeepAliveWithoutCallChannel(userHost).Intercept(tracingInterceptor))
            );

            services.AddSingleton(new LocalizationService.LocalizationServiceClient(
                CreateKeepAliveWithoutCallChannel(userHost).Intercept(tracingInterceptor))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = async context =>
                {
                    if (Regex.IsMatch(context.Request.Path.Value, "^/api/", RegexOptions.IgnoreCase))
                        await HandleApiException(context);
                    else if (env.IsDevelopment())
                        app.UseDeveloperExceptionPage();
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
            app.UseCors(CorsPolicy);
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllerRoute( "default", "{controller}/{action=Index}/{id?}" ));
            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                {
                    spa.Options.SourcePath = "ClientApp";
                    spa.UseReactDevelopmentServer("start");
                }
                else if (env.IsProduction()) spa.Options.SourcePath = "ClientApp/build";
            });
        }

        private Channel CreateKeepAliveWithoutCallChannel(string hostString)
        {
            return new Channel(hostString, ChannelCredentials.Insecure, new List<ChannelOption>
            {
                new ChannelOption("grpc.keepalive_permit_without_calls", 1),
                new ChannelOption("grpc.http2.max_pings_without_data", 0),
                new ChannelOption("grpc.keepalive_time_ms", 1000 * 60 * 20),
                new ChannelOption("grpc.max_receive_message_length", -1),
                new ChannelOption("grpc.max_send_message_length", -1)
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