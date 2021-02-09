using k8s;
using Microsoft.Extensions.Logging;
using System;

namespace Neuroglia.K8s.Eventing
{

    /// <summary>
    /// Represents the base class for all <see cref="IController"/> implementations
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to control</typeparam>
    public abstract class Controller<TResource>
        : IController<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Initializes a new <see cref="Controller{TResource}"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="kubernetes">The service used to interact with the Kubernetes API</param>
        /// <param name="resources">The service used to watch <see cref="ICustomResource"/>s of the specified type</param>
        protected Controller(ILoggerFactory loggerFactory, IKubernetes kubernetes, ICustomResourceWatcher<TResource> resources)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.Kubernetes = kubernetes;
            this.Resources = resources;
            this.Subscription = this.Resources.Subscribe(this.OnEvent, this.OnError, this.OnCompleted);
            this.ResourceDefinition = new TResource().Definition;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to interact with the Kubernetes API
        /// </summary>
        protected IKubernetes Kubernetes { get; }

        /// <summary>
        /// Gets the <see cref="CustomResourceDefinition"/> of the controlled <see cref="ICustomResource"/> type
        /// </summary>
        protected ICustomResourceDefinition ResourceDefinition { get; }

        /// <summary>
        /// Gets the service used to watch subscriptions
        /// </summary>
        protected ICustomResourceWatcher<TResource> Resources { get; }

        /// <summary>
        /// Gets the current subscription watch
        /// </summary>
        protected IDisposable Subscription { get; private set; }

        /// <summary>
        /// Handles the specified <see cref="IResourceEvent{TResource}"/>
        /// </summary>
        /// <param name="e">The <see cref="IResourceEvent{TResource}"/> to handle</param>
        protected virtual void OnEvent(IResourceEvent<TResource> e)
        {
            this.Logger.LogDebug("An event of type '{type}' has been received concerning a resource of kind '{kind}' (apiVersion={apiVersion})", e.Type, this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion);
        }

        /// <summary>
        /// Handles an <see cref="Exception"/> that has occured while watching the <see cref="ICustomResource"/>s of the specified type
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/></param>
        protected virtual void OnError(Exception ex)
        {
            this.Logger.LogError("An exception has occured while watching custom resources of kind '{kind}' (apiVersion={apiVersion})", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion);
        }

        /// <summary>
        /// Handles the completion of the subscription's stream
        /// </summary>
        protected virtual void OnCompleted()
        {
            this.Logger.LogDebug("Completed watching custom resources of kind '{kind}' (apiVersion={apiVersion})", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion);
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="Controller{TResource}"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="Controller{TResource}"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    this.Subscription?.Dispose();
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
