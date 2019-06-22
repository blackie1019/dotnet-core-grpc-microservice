#region

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

#endregion

namespace MockSite.Common.Core.Utilities
{
    public class ConsulSettingHelper
    {
        private static readonly Lazy<ConsulSettingHelper> _lazy =
            new Lazy<ConsulSettingHelper>(() => new ConsulSettingHelper());

        public static ConsulSettingHelper Instance => _lazy.Value;

        private readonly IConfigurationRoot _configuration;

        private ConsulSettingHelper()
        {
            //先建立Local Config
            var builder = new ConfigurationBuilder()
                .SetFileProvider(
                    new PhysicalFileProvider(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));

            builder.AddJsonFile("appsettings.json");
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            {
                builder.AddJsonFile(
                    "appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json",
                    true);
            }

            _configuration = builder.Build();

            // 從 consul kv store 取得 config
            var configProvider = ConfigHelper.GetConfig(
                $"http://{GetValueFromKey("Consul:IP")}:{GetValueFromKey("Consul:Port")}/v1/kv/",
                GetValueFromKey("Consul:Module").Split(','));

            // 將 Consul config 塞入 _configuration 中
            builder = new ConfigurationBuilder()
                .AddJsonFile(configProvider, "none.json", true, false);
            _configuration = builder.Build();
        }

        public string GetValueFromKey(string key)
        {
            return _configuration.GetSection(key).Value;
        }
    }
}