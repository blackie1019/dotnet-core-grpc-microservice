#region

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

#endregion

namespace MockSite.Common.Core.Utilities
{
    public class AppSettingsHelper
    {
        private const string DefaultLoadFile = "appsettings.json";

        private IConfigurationRoot _configuration;

        private readonly IConfigurationBuilder _builder;

        private static readonly Lazy<AppSettingsHelper> _lazy =
            new Lazy<AppSettingsHelper>(() => new AppSettingsHelper());

        public static AppSettingsHelper Instance
        {
            get { return _lazy.Value; }
        }

        private AppSettingsHelper()
        {
            var basePath = Directory.GetCurrentDirectory();
            _builder = new ConfigurationBuilder()
                .SetFileProvider(new PhysicalFileProvider(basePath))
                .AddJsonFile(DefaultLoadFile);

            _configuration = _builder.Build();
        }

        public void LoadAppendFile(string appendFile)
        {
            _builder.AddJsonFile(appendFile);

            _configuration = _builder.Build();
        }

        public string GetValueFromKey(string key)
        {
            return _configuration.GetSection(key).Value;
        }
    }
}