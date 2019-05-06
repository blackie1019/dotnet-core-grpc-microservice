using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace MockSite.Common.Logging.Utilities.LogProvider.Serilog
{
    internal class ApplicationNameHelper
    {
        private const string DefaultLoadFile = "appsettings.json";

        private IConfigurationRoot _configuration;

        private readonly IConfigurationBuilder _builder;

        private static readonly Lazy<ApplicationNameHelper> Lazy =
            new Lazy<ApplicationNameHelper>(() => new ApplicationNameHelper());

        public static ApplicationNameHelper Instance
        {
            get { return Lazy.Value; }
        }

        ApplicationNameHelper()
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