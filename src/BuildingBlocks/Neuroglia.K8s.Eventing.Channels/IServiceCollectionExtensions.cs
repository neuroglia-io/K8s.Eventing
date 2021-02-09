using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neuroglia.K8s.Eventing.Channels.Services;
using System;

namespace Neuroglia.K8s.Eventing.Channels
{
    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures an <see cref="ISubscriptionManager"/> of the specified type
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="subscriptionManagerType">The type of the <see cref="ISubscriptionManager"/> to use</param>
        /// <param name="configuration">An <see cref="Action{T}"/> used to configure the options of the <see cref="ISubscriptionManager"/> to use</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddSubscriptionManager(this IServiceCollection services, Type subscriptionManagerType, Action<ISubscriptionManagerOptionsBuilder> configuration)
        {
            ISubscriptionManagerOptionsBuilder optionsBuilder = new SubscriptionManagerOptionsBuilder();
            configuration(optionsBuilder);
            SubscriptionManagerOptions options = optionsBuilder.Build();
            services.AddSingleton(Options.Create(options));
            services.AddSingleton(typeof(ISubscriptionManager), subscriptionManagerType);
            return services;
        }

        /// <summary>
        /// Adds and configures an <see cref="ISubscriptionManager"/> of the specified type
        /// </summary>
        /// <typeparam name="TManager">The type of the <see cref="ISubscriptionManager"/> to use</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configuration">An <see cref="Action{T}"/> used to configure the options of the <see cref="ISubscriptionManager"/> to use</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddSubscriptionManager<TManager>(this IServiceCollection services, Action<ISubscriptionManagerOptionsBuilder> configuration)
            where TManager : class, ISubscriptionManager
        {
            services.AddSubscriptionManager(typeof(TManager), configuration);
            return services;
        }

        /// <summary>
        /// Adds and configures an <see cref="SubscriptionManager"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configuration">An <see cref="Action{T}"/> used to configure the options of the <see cref="ISubscriptionManager"/> to use</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddSubscriptionManager(this IServiceCollection services, Action<ISubscriptionManagerOptionsBuilder> configuration)
        {
            services.AddSubscriptionManager(typeof(SubscriptionManager), configuration);
            return services;
        }

    }

}
