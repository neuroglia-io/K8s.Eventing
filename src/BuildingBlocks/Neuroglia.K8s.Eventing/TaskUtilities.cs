using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s.Eventing
{

    /// <summary>
    /// Defines utilities for <see cref="Task"/>s
    /// </summary>
    public static class TaskUtilities
    {

        /// <summary>
        /// Waits for the first <see cref="Task"/> to produce a result that satisfies the specified predicate
        /// </summary>
        /// <typeparam name="T">The type of expected result</typeparam>
        /// <param name="taskFactories">An <see cref="IEnumerable{T}"/> containing the <see cref="Func{T, TResult}"/> used to create the <see cref="Task"/>s to run</param>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>The first matching result</returns>
        public static Task<T> WhenAny<T>(this IEnumerable<Func<CancellationToken, Task<T>>> taskFactories, Func<T, bool> predicate)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
            int total = taskFactories.Count();
            int completed = 0;
            foreach (Func<CancellationToken, Task<T>> taskFactory in taskFactories)
            {
                taskFactory(cancellationTokenSource.Token).ContinueWith(t =>
                {
                    if (t.Exception != null)
                        return;
                    else if (predicate(t.Result))
                    {
                        cancellationTokenSource.Cancel();
                        taskCompletionSource.TrySetResult(t.Result);
                    }
                    if (Interlocked.Increment(ref completed) == total)
                        taskCompletionSource.TrySetResult(default);

                }, cancellationTokenSource.Token);
            }
            return taskCompletionSource.Task;
        }

    }

}
