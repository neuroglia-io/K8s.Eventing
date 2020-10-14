using MediatR;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s
    /// </summary>
    /// <typeparam name="TCommand">The type of <see cref="ICommand"/>s to handle</typeparam>
    public interface ICommandHandler<TCommand>
        : IRequestHandler<TCommand, IOperationResult>
        where TCommand : ICommand<IOperationResult>
    {



    }

    /// <summary>
    /// Defines the fundamentals of a service used to handle <see cref="ICommand"/>s
    /// </summary>
    /// <typeparam name="TCommand">The type of <see cref="ICommand"/>s to handle</typeparam>
    /// <typeparam name="T">The type of data returned by the <see cref="ICommand"/>s to handle</typeparam>
    public interface ICommandHandler<TCommand, T>
        : IRequestHandler<TCommand, IOperationResult<T>>
        where TCommand : ICommand<IOperationResult<T>, T>
    {



    }

}
