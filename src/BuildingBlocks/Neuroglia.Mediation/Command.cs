namespace Neuroglia.Mediation
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ICommand{TResult}"/> interface
    /// </summary>
    public abstract class Command
        : ICommand<IOperationResult>
    {



    }

    /// <summary>
    /// Represents the default implementation of the <see cref="ICommand{TResult, T}"/> interface
    /// </summary>
    public abstract class Command<T>
        : ICommand<IOperationResult<T>, T>
    {



    }

}
