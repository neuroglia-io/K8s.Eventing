using Neuroglia.AutoMapper;
using Neuroglia.K8s.Eventing.Gateway.Application.Commands;
using Neuroglia.K8s.Eventing.Gateway.Integration.Commands;

namespace Neuroglia.K8s.Eventing.Gateway.Application.Mapping.Configuration
{

    internal class CreateSubscriptionCommandMappingConfiguration
        : IMappingConfiguration<CreateSubscriptionCommandDto, CreateSubscriptionCommand>
    {

        void IMappingConfiguration<CreateSubscriptionCommandDto, CreateSubscriptionCommand>.Configure(global::AutoMapper.IMappingExpression<CreateSubscriptionCommandDto, CreateSubscriptionCommand> mapping)
        {
            
        }

    }

}
