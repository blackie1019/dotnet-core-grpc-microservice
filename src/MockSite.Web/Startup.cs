using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MockSite.Common.Core.Constants.DomainService;
using MockSite.Common.Core.Utilities;
using MockSite.Web.Consts;
using MockSite.Web.Services;
using OpenTracing;
using OpenTracing.Contrib.Grpc.Interceptors;
using OpenTracing.Util;
using Swashbuckle.AspNetCore.Swagger;

namespace MockSite.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //加入 OpenTracing
            services.AddOpenTracing();
            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = "MockSite.Web";
                var tracer = new Tracer.Builder(serviceName)
                    .WithSampler(new ConstSampler(true))
                    .Build();
                //註冊 Jaeger tracer
                GlobalTracer.Register(tracer);
 
                return tracer;
            });
            
            
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "MockSite API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            
            // configure jwt authentication
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettingsHelper.Instance.GetValueFromKey(JwtSettingConsts.SecretKey)));
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //what to validate
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        //setup validate data
                        ValidIssuer = AppSettingsHelper.Instance.GetValueFromKey(JwtSettingConsts.IssuerKey),
                        ValidAudience = AppSettingsHelper.Instance.GetValueFromKey(JwtSettingConsts.AudienceKey),
                        IssuerSigningKey = symmetricSecurityKey
                    };
                });
            

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            
            // Register gRPC Service
            var userHost = $"{ConsulSettingHelper.Instance.GetValueFromKey(HostNameConst.TestKey)}:{ConsulSettingHelper.Instance.GetValueFromKey(PortConst.TestKey)}";

            var tracerInstance = GlobalTracer.Instance;
            ClientTracingInterceptor tracingInterceptor = new ClientTracingInterceptor(tracerInstance);

            services.AddSingleton(
                new MockSite.Message.UserService.UserServiceClient(CreateKeepAliveWithoutCallChannel(userHost).Intercept(tracingInterceptor)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });

            if (env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";
                    spa.UseReactDevelopmentServer("start");
                });
            }
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
    }
}