using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Kafka;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neuroglia.K8s.Eventing.Channels.Commands;
using Neuroglia.K8s.Eventing.Channels.Services;
using Neuroglia.K8s.Eventing.Extensions;
using Neuroglia.K8s.Eventing.Kafka.Channel.Application.Configuration;
using Neuroglia.K8s.Eventing.Resources;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing.Kafka.Channel.Application.Services
{

    /// <summary>
    /// Represents the Kafka implementation of the <see cref="IEventSource"/> interface
    /// </summary>
    public class Subscription
        : ISubscription
    {

        private CancellationTokenSource _CancellationTokenSource = new CancellationTokenSource();
        private Task _ExecutingTask;

        /// <summary>
        /// Initializes a new <see cref="Subscription"/>
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="consumerFactory">The service used to create <see cref="IConsumer{TKey, TValue}"/> instances</param>
        /// <param name="topicMapper">The service used to map Kafka topics</param>
        /// <param name="formatter">The service used to format <see cref="CloudEvent"/>s</param>
        /// <param name="mediator">The service used to mediate calls</param>
        /// <param name="options">The service used to access the current <see cref="KafkaOptions"/></param>
        /// <param name="name">The <see cref="Subscription"/>'s name</param>
        /// <param name="spec">The <see cref="Subscription"/>'s <see cref="V1SubscriptionSpec"/></param>
        public Subscription(ILogger<Subscription> logger, IKafkaConsumerFactory consumerFactory, IKafkaTopicMapper topicMapper, ICloudEventFormatter formatter, IMediator mediator, IOptions<KafkaOptions> options, string name, V1SubscriptionSpec spec)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Logger = logger;
            this.EventFormatter = formatter;
            this.Mediator = mediator;
            this.Options = options.Value;
            this.Name = name;
            this.Spec = spec;
            this.Kafka = consumerFactory.Create<string, byte[]>(config =>
            {
                string groupId = string.Empty;
                config.AutoOffsetReset = AutoOffsetReset.Latest;
                if (spec.Stream != null)
                {
                    if (spec.Stream.IsDurable)
                    {
                        groupId = this.Name;
                        config.EnableAutoCommit = true;
                    }
                    if (spec.Stream.Offset.HasValue
                        && spec.Stream.Offset.Value == 0)
                    {
                        config.AutoOffsetReset = AutoOffsetReset.Earliest;
                    }
                }
                else
                {
                    config.EnableAutoCommit = false;
                }
                if (string.IsNullOrWhiteSpace(groupId))
                    groupId = this.Options.DefaultConsumerGroupId;
                config.GroupId = groupId;
            });
            this.TopicMapper = topicMapper;
            this.EventStream = System.Threading.Channels.Channel.CreateUnbounded<CloudEvent>();
            _ = this.StartAsync();
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to consume Kafka events
        /// </summary>
        protected IConsumer<string, byte[]> Kafka { get; }

        /// <summary>
        /// Gets the service used to map Kafka topics
        /// </summary>
        protected IKafkaTopicMapper TopicMapper { get; }

        /// <summary>
        /// Gets the service used to format <see cref="CloudEvent"/>s
        /// </summary>
        protected ICloudEventFormatter EventFormatter { get; }

        /// <summary>
        /// Gets the service used to mediate calls
        /// </summary>
        protected IMediator Mediator { get; }

        /// <summary>
        /// Gets the current <see cref="KafkaOptions"/>
        /// </summary>
        protected KafkaOptions Options { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public V1SubscriptionSpec Spec { get; }

        /// <summary>
        /// Gets a <see cref="Channel{T}"/> used to stream incoming <see cref="CloudEvent"/>s
        /// </summary>
        protected Channel<CloudEvent> EventStream { get; }

        /// <summary>
        /// Starts the <see cref="Subscription"/>
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual Task StartAsync(CancellationToken cancellationToken = default)
        {
            string topic = this.TopicMapper.Map(this.Spec);
            this.Logger.LogInformation("Starting subscription with name {name} (Kafka topic '{topic}')...", this.Name, topic);
            if ((this.Spec.Stream != null
                && this.Spec.Stream.IsDurable)
                || topic.Contains('*'))
            {
                //if subscription is durable or if topic is a regex, just subscribe
                this.Kafka.Subscribe(topic);
            }
            else
            {
                //otherwise, seek the desired position
                Offset offset = Offset.End;
                if (this.Spec.Stream != null
                    && this.Spec.Stream.Offset.HasValue)
                {
                    if (this.Spec.Stream.Offset == 0)
                        offset = Offset.Beginning;
                    else
                        offset = new Offset(this.Spec.Stream.Offset.Value);
                }
                TopicPartitionOffset topicPartition = new TopicPartitionOffset(new TopicPartition(topic, Partition.Any), offset);
                this.Kafka.Assign(topicPartition);
            }
            this._ExecutingTask = Task.WhenAll(Task.Run(() => this.ConsumeAsync()), Task.Run(() => this.DispatchAsync()));
            this.Logger.LogInformation("The subscription with name {name} (Kafka topic '{topic}') is now running", this.Name, topic);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Consumes Kafka events
        /// </summary>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task ConsumeAsync()
        {
            this.Logger.LogInformation("Consuming Kafka messages for subscription '{subscription}'...", this.Name);
            while (!this._CancellationTokenSource.IsCancellationRequested) 
            {
                try
                {
                    ConsumeResult<string, byte[]> consumeResult = this.Kafka.Consume(this._CancellationTokenSource.Token);
                    if (consumeResult == null)
                        continue;
                    this.Logger.LogInformation("A Kafka message with key '{key}' and topic '{topic}' has been received on subscription '{subscription}'.", consumeResult.Message.Key, consumeResult.Topic, this.Name);
                    CloudEvent e = consumeResult.Message.ToCloudEvent(this.EventFormatter, new LongSequenceExtension(consumeResult.Offset));
                    this.Logger.LogInformation("The cloud event with id '{eventId}' has been successfully extracted from the kafka message with key '{key}'", e.Id, consumeResult.Message.Key);
                    await this.EventStream.Writer.WriteAsync(e, this._CancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An error occured while consuming an incoming Kafka event.{Environment.NewLine}Details: {{ex}}", ex.Message);
                }
            }
            this.Logger.LogInformation("Stopped consuming Kafka messages for subscription '{subscription}'", this.Name);
        }

        /// <summary>
        /// Dispatches pending <see cref="CloudEvent"/>s
        /// </summary>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task DispatchAsync()
        {
            this.Logger.LogInformation("Dispatching pending outbound cloud events for subscription '{subscription}'...", this.Name);
            while (!this._CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    CloudEvent e = await this.EventStream.Reader.ReadAsync(this._CancellationTokenSource.Token);
                    this.Logger.LogInformation("Dispatching the cloud event with id '{eventId}'...", e.Id);
                    await this.Mediator.Send(new DispatchEventCommand(e), this._CancellationTokenSource.Token);
                    this.Logger.LogInformation("The cloud event with id '{eventId}' has been successfully dispatched", e.Id);
                }
                catch(Exception ex)
                {
                    this.Logger.LogError($"An error occured while dispatching an incoming Kafka event.{Environment.NewLine}Details: {{ex}}", ex.Message);
                }
            }
            this.Logger.LogInformation("Stopped dispatching pending outbound cloud events for subscription '{subscription}'", this.Name);
        }

        /// <summary>
        /// Stops the <see cref="Subscription"/>
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (this._ExecutingTask == null)
                return;
            try
            {
                this.Kafka.Unsubscribe();
                this._CancellationTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(this._ExecutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
                this._ExecutingTask = null;
            }
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="SubscriptionManager"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="SubscriptionManager"/> has been disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    this.StopAsync(default).GetAwaiter().GetResult();
                    this.Kafka?.Dispose();
                }
                this._Disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

}
