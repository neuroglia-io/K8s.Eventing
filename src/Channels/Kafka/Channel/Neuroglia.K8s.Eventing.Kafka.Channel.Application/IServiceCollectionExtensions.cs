using CloudNative.CloudEvents;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neuroglia.AspNetCore.Http;
using Neuroglia.K8s.Eventing.Channels;
using Neuroglia.K8s.Eventing.Channels.Commands;
using Neuroglia.K8s.Eventing.Channels.Services;
using Neuroglia.K8s.Eventing.Kafka.Channel.Application.Configuration;
using Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services;
using Neuroglia.K8s.Eventing.Resources;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures all services required by the application
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configuration">The current <see cref="IConfiguration"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            ApplicationOptions options = new ApplicationOptions();
            configuration.Bind(options);
            services.AddSingleton(Options.Create(options));
            services.AddSingleton(Options.Create(options.Kafka));

            services.AddMediatR(typeof(SubscribeCommand).Assembly);
            services.AddSingleton<ICloudEventFormatter, JsonEventFormatter>();

            services.AddKubernetesClient();
            services.AddCustomResourceWatcher<V1SubscriptionResource>();
            services.AddHttpClient<IEventDispatcher, EventDispatcher>();
            services.AddSingleton<IEventIngestor, EventIngestor>();
            services.AddSubscriptionManager(builder => builder.ForChannel(options.Channel));
            services.AddSingleton<ISubscriptionFactory, SubscriptionFactory>();
            services.AddSingleton<IKafkaTopicMapper, KafkaTopicMapper>();
            services.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
            services.AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();

            services.AddHealthChecks();
            services.AddIstioHeadersPropagation();
            return services;
        }

    }

}
