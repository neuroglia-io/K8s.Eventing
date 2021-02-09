using System;

namespace Neuroglia.K8s.Eventing
{

    /// <summary>
    /// Defines the fundamentals of a service used to control a Kubernetes resource of the specified type
    /// </summary>
    public interface IController
        : IDisposable
    {



    }

    /// <summary>
    /// Defines the fundamentals of a service used to control a Kubernetes resource of the specified type
    /// </summary>
    /// <typeparam name="TResource">The type of Kubernetes resource to watch</typeparam>
    public interface IController<TResource>
        : IController
        where TResource : class, ICustomResource, new()
    {



    }

}
