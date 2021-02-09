namespace Neuroglia.K8s.Eventing.Resources
{

    /// <summary>
    /// Defines the fundamentals of a channel <see cref="ICustomResource"/>
    /// </summary>
    public interface IV1ChannelResource<TSpec, TStatus>
        : ICustomResource<TSpec, TStatus>
        where TSpec : class, IV1ChannelSpec, new()
        where TStatus : class, IV1ChannelStatus, new()
    {



    }

}
