#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace MockSite.Common.Core.Utilities
{
    public static class ConfigHelper
    {
        [SuppressMessage("ReSharper", "S112")]
        public static InMemoryFileProvider GetConfig(string consulKvApi, IEnumerable<string> modules)
        {
            var httpClient = new HttpClient();
            if (string.IsNullOrWhiteSpace(consulKvApi) || !Uri.IsWellFormedUriString(consulKvApi, UriKind.Absolute))
                return null;

            httpClient.BaseAddress = new Uri(consulKvApi);

            JObject configs = null;

            Dictionary<string, string> configKeys = null;
            foreach (var module in modules)
            {
                var httpResult = httpClient.GetAsync(module).ConfigureAwait(false).GetAwaiter()
                    .GetResult();

                var content = httpResult.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                if (string.IsNullOrWhiteSpace(content)) continue;
                var contentValue = JsonConvert.DeserializeObject<List<ConsulKv>>(content).FirstOrDefault();
                var decodedBytes = Convert.FromBase64String(contentValue?.Value);
                var decodedTxt = Encoding.UTF8.GetString(decodedBytes);
                var tmpConfig = new JObject
                {
                    new JProperty(module, JsonConvert.DeserializeObject(decodedTxt))
                };

                if (configs is null)
                    configs = tmpConfig;
                else
                {
                    var tmpKeys = GetAllKeys(tmpConfig.ToString());

                    if (configKeys.Keys.Intersect(tmpKeys.Keys).Any())
                        throw new ApplicationException("duplicate key");

                    configs.Merge(tmpConfig);
                }

                configKeys = GetAllKeys(configs.ToString());
            }

            if (configs == null)
                return null;

            var memoryFileProvider = new InMemoryFileProvider(configs.ToString());
            return memoryFileProvider;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static Dictionary<string, string> GetAllKeys(string decodedTxt)
        {
            var jsonObject = JObject.Parse(decodedTxt);
            var jTokens = jsonObject.Descendants().Where(p => !p.Any());
            var tmpKeys = jTokens.Aggregate(new Dictionary<string, string>(),
                (properties, jToken) =>
                {
                    properties.Add(jToken.Path, jToken.ToString());
                    return properties;
                });
            return tmpKeys;
        }
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ConsulKv
    {
        public string LockIndex { get; set; }

        public string Key { get; set; }

        public int Flags { get; set; }

        public string Value { get; set; }

        public int CreateIndex { get; set; }

        public int ModifyIndex { get; set; }
    }

    public class InMemoryFileProvider : IFileProvider
    {
        private class InMemoryFile : IFileInfo
        {
            private readonly byte[] _data;

            public InMemoryFile(string json) => _data = Encoding.UTF8.GetBytes(json);

            public Stream CreateReadStream() => new MemoryStream(_data);

            public bool Exists { get; } = true;

            public long Length => _data.Length;

            public string PhysicalPath { get; } = string.Empty;

            public string Name { get; } = string.Empty;

            public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;

            public bool IsDirectory { get; } = false;
        }

        private readonly IFileInfo _fileInfo;

        public InMemoryFileProvider(string json) => _fileInfo = new InMemoryFile(json);

        public IFileInfo GetFileInfo(string subPath) => _fileInfo;

        [SuppressMessage("ReSharper", "S1168")]
        public IDirectoryContents GetDirectoryContents(string subPath) => null;

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}