using AutoMapper;
using Neuroglia.AutoMapper;
using Neuroglia.K8s.Eventing.Gateway.Application.Mapping.Configuration;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Mapping
{

    /// <summary>
    /// Represents the application's <see cref="Profile"/>
    /// </summary>
    public class MappingProfile
        : Profile
    {

        /// <summary>
        /// Initializes a new <see cref="MappingProfile"/>
        /// </summary>
        public MappingProfile()
        {
            this.ApplyConfiguration(new CreateSubscriptionCommandMappingConfiguration());
        }

    }

}
