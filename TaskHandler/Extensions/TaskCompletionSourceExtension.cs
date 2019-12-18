using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskHandler.Extensions
{
    public static class TaskCompletionSourceExtension
    {
        public static async Task<TResult> WaitAsync<TResult>(this TaskCompletionSource<TResult> tcs, CancellationToken cancelToken, int timeoutMs = Timeout.Infinite, bool updateTcs = false)
        {
            var overrideTcs = new TaskCompletionSource<TResult>();

            using (var timeoutCancelTokenSource = (timeoutMs <= 0 || timeoutMs == Timeout.Infinite) ? null : new CancellationTokenSource(timeoutMs))
            {
                var timeoutToken = timeoutCancelTokenSource?.Token ?? CancellationToken.None;

                using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancelToken, timeoutToken))
                {
                    void CancelTcs()
                    {
                        if (updateTcs && !tcs.Task.IsCompleted)
                        {
                            // ReSharper disable once AccessToDisposedClosure (in this case, CancelTcs will never be called outside the using)
                            if (timeoutCancelTokenSource?.IsCancellationRequested ?? false)
                            {
                                tcs.TrySetException(new TimeoutException($"WaitAsync timed out after {timeoutMs}ms"));
                            }
                            else
                            {
                                tcs.TrySetCanceled();
                            }
                        }

                        overrideTcs.TrySetResult(default(TResult));
                    }

                    using (linkedTokenSource.Token.Register(CancelTcs))
                    {
                        try
                        {
                            await Task.WhenAny(tcs.Task, overrideTcs.Task);
                        }
                        catch
                        {
                        }

                        if (tcs.Task.IsCompleted)
                        {
                            // We do another await here so that if the tcs.Task has faulted or has been canceled we won't wrap those exceptions
                            // in a nested exception.  While technically accessing the tcs.Task.Result will generate the same exception the
                            // exception will be wrapped in a nested exception.  We don't want that nesting so we just await.
                            await tcs.Task;
                            return tcs.Task.Result;
                        }

                        // It wasn't the tcs.Task that got us our of the above WhenAny so go ahead and timeout or cancel the operation.
                        //
                        if (timeoutCancelTokenSource?.IsCancellationRequested ?? false)
                        {
                            throw new TimeoutException($"WaitAsync timed out after {timeoutMs}ms");
                        }

                        throw new OperationCanceledException();
                    }
                }
            }
        }
    }
}
