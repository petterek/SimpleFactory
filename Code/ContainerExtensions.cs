using System;

namespace SimpleFactory
{
    public static class ContainerExtensions
    {
        public static TType CreateInstance<TType>(this IServiceProvider serviceProvider)
        {
            return (TType)serviceProvider.GetService(typeof(TType));
        }
        public static TType GetService<TType>(this IServiceProvider serviceProvider)
        {
            return (TType)serviceProvider.GetService(typeof(TType));
        }
    }
}