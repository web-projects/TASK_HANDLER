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
                                tcs.TrySetException(new TimeoutException($"operation timed out after {timeoutMs}ms"));
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
                            await tcs.Task;
                            return tcs.Task.Result;
                        }

                        // operation timed out
                        if (timeoutCancelTokenSource?.IsCancellationRequested ?? false)
                        {
                            throw new TimeoutException($"operation timed out after {timeoutMs}ms");
                        }

                        throw new OperationCanceledException();
                    }
                }
            }
        }
    }
}
