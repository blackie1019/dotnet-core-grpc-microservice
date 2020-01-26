using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using MockSite.Common.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MockSite.Common.Core.Utilities
{
    public class ConsulConfigProvider
    {
        public static InMemoryFileProvider LoadConsulConfig(string consulKvApi, IEnumerable<string> modules)
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
                var contentValue = JsonConvert.DeserializeObject<List<ConsulKvModel>>(content).FirstOrDefault();

                if (contentValue == null) continue;
                var decodedBytes = Convert.FromBase64String(contentValue.Value);
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
            
            var memoryFileProvider = new InMemoryFileProvider(configs.ToString());

            return memoryFileProvider;
        }
        
        
        private static Dictionary<string, string> GetAllKeys(string decodedTxt)
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
}