using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds the specified command behavior type
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="behaviorType">The type of the command behavior type to add</param>
        /// <param name="assemblyToScan">The <see cref="Assembly"/> to scan for <see cref="ICommand"/>s</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCommandBehavior(this IServiceCollection services, Type behaviorType, Assembly assemblyToScan)
        {
            foreach(Type type in assemblyToScan.GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t)))
            {
                Type commandType = type;
                Type operationResultType = type.IsGenericType ? typeof(IOperationResult<>).MakeGenericType(type.GetGenericArguments()[0]) : typeof(IOperationResult);
                Type serviceType = typeof(IPipelineBehavior<,>).MakeGenericType(commandType, operationResultType);
                Type implementationType = behaviorType.MakeGenericType(commandType, operationResultType);
                services.AddTransient(serviceType, implementationType);
            }
            return services;
        }

    }

}
