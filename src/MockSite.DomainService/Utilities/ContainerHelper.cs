using System;
using MockSite.Common.Core.Utilities;
using MockSite.Core.Repositories;
using MockSite.Core.Services;
using Unity;
using Unity.Interception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;

namespace MockSite.DomainService.Utilities
{
    public class ContainerHelper
    {
        private const string PerformanceInterceptorKey = "Aop:PerformanceInterceptor";
        public static readonly ContainerHelper Instance = new ContainerHelper();

        public readonly IUnityContainer Container;
        
        private ContainerHelper()
        {
            Container = new UnityContainer();
            if (Convert.ToBoolean(AppSettingsHelper.Instance.GetValueFromKey(PerformanceInterceptorKey)))
                Container.AddNewExtension<Interception>();

            RegisterMapping();
        }

        private void RegisterMapping()
        {
            Container.RegisterSingleton<IUserService,UserService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<Performanceinterceptor>());
            Container.RegisterInstance<IRepository>(new UserRepository());
        }
    }
}