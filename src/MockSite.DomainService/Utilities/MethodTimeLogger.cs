#region

using System;
using System.Reflection;
using MockSite.Common.Logging.Utilities;
using Newtonsoft.Json;

#endregion

namespace MockSite.DomainService.Utilities
{
    public class MethodTimeLogger
    {
        public static void Log(MethodBase methodBase, long milliseconds, string message)
        {
            try
            {
                var perfDetail = LoggerHelper.Instance.GetPerformanceDetail();
                perfDetail.Target = $"{methodBase.DeclaringType.Name}/{methodBase.Name}";
                perfDetail.Duration = milliseconds;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    perfDetail.Argument = JsonConvert.DeserializeObject(message);
                }

                LoggerHelper.Instance.Performance(perfDetail);
            }
            catch (Exception ex)
            {
                LoggerHelper.Instance.Error(ex);
            }
        }
    }
}