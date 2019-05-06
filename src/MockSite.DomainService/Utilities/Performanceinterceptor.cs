using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using MockSite.Common.Logging.Utilities;
using Newtonsoft.Json;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace MockSite.DomainService.Utilities
{
    public class Performanceinterceptor : IInterceptionBehavior
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            // Before invoking the method on the original target.   
            var perfDetail = LoggerHelper.Instance.GetPerformanceDetail();
            var method = input.MethodBase as MethodInfo;
            if (input.Arguments.Count > 0)
            {
                var paramterDic = new Dictionary<string, string>();
                for (var index = 0; index < input.Arguments.Count; index++)
                {
                    var name = input.Arguments.ParameterName(index);
                    var value = input.Arguments[index];
                    paramterDic.Add(name, JsonConvert.SerializeObject(value));
                }

                perfDetail.Parameter = paramterDic;
            }

            perfDetail.Target = $"{input.Target.GetType().Name}/{method.Name}";

            var sw = new Stopwatch();
            sw.Start();

            // Invoke the next behavior in the chain. 
            var result = getNext()(input, getNext);

            // After invoking the method on the original target. 
            sw.Stop();

            perfDetail.Duration = sw.ElapsedMilliseconds;
            LoggerHelper.Instance.Performance(perfDetail);

            return result;
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public bool WillExecute => true;
    }
}