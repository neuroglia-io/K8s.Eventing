using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace Neuroglia.AutoMapper
{
    /// <summary>
    /// Defines extensions for <see cref="Profile"/>s
    /// </summary>
    public static class ProfileExtensions
    {

        private static MethodInfo GenericApplyConfigurationMethod = typeof(ProfileExtensions)
            .GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => m.Name == nameof(ApplyConfiguration) && m.IsGenericMethod)
            .First();

        /// <summary>
        /// Applies the specified configuration
        /// </summary>
        /// <typeparam name="TSource">The type to map from</typeparam>
        /// <typeparam name="TDestination">The type to map to</typeparam>
        /// <param name="profile">The <see cref="Profile"/> to configure</param>
        /// <param name="configuration">The <see cref="IMappingConfiguration{TSource, TDestination}"/> to apply</param>
        public static void ApplyConfiguration<TSource, TDestination>(this Profile profile, IMappingConfiguration<TSource, TDestination> configuration)
        {
            configuration.Configure(profile.CreateMap<TSource, TDestination>());
        }

        /// <summary>
        /// Applies the specified <see cref="IMappingConfiguration"/>
        /// </summary>
        /// <param name="profile">The <see cref="Profile"/> to configure</param>
        /// <param name="configuration">The <see cref="IMappingConfiguration"/> to apply</param>
        public static void ApplyConfiguration(this Profile profile, IMappingConfiguration configuration)
        {
            foreach (Type configurationType in configuration.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMappingConfiguration<,>)))
            {
                Type sourceType = configurationType.GetGenericArguments()[0];
                Type destinationType = configurationType.GetGenericArguments()[1];
                GenericApplyConfigurationMethod.MakeGenericMethod(sourceType, destinationType).Invoke(null, new object[] { profile, configuration });
            }
        }

    }

}
