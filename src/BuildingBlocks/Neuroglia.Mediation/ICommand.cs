using MediatR;

namespace Neuroglia.Mediation
{

    /// <summary>
    /// Defines the fundamentals of a command
    /// </summary>
    public interface ICommand
    {


    }

    /// <summary>
    /// Defines the fundamentals of a command
    /// </summary>
    /// <typeparam name="TResult">The type of expected <see cref="IOperationResult"/></typeparam>
    public interface ICommand<TResult>
        : ICommand, IRequest<TResult>
        where TResult : IOperationResult
    {



    }

    /// <summary>
    /// Defines the fundamentals of a command
    /// </summary>
    /// <typeparam name="TResult">The type of expected <see cref="IOperationResult"/></typeparam>
    /// <typeparam name="T">The type of data returned by the command</typeparam>
    public interface ICommand<TResult, T>
        : ICommand, ICommand<TResult>
        where TResult : IOperationResult<T>
    {



    }

}
