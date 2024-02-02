using System;
using System.Threading;
using System.Threading.Tasks;
using WorkItems.Model;

namespace WorkItems.Utility;

internal static class TaskUtility
{
    public static async void FileAndForget(Func<Task> func)
    {
        try
        {
            await func?.Invoke();
        }
        catch
        {
            // Forget about it, maybe log telemetry if that ever gets added
        }
    }

    public static async void FileOnceAndForget(Wrapper<CancellationTokenSource> ctsWrapper, Func<CancellationTokenSource, Task> func)
    {
        if (ctsWrapper.Value != null)
        {
            // Already doing work
            return;
        }

        CancellationTokenSource originalCancellationTokenSource = new();
        ctsWrapper.Value = originalCancellationTokenSource;

        try
        {
            await func?.Invoke(originalCancellationTokenSource);
        }
        finally
        {
            if (ctsWrapper.Value == originalCancellationTokenSource)
            {
                ctsWrapper.Value = null;
            }
        }
    }

    public static void FileOnceAndForget(
        Wrapper<CancellationTokenSource> ctsWrapper,
        IProgressBar progressBar,
        IInfoBar infoBar,
        Func<WorkData, CancellationTokenSource, Task> func,
        string workText = null,
        double initialProgress = 1)
    {
        TaskUtility.FileOnceAndForget(ctsWrapper, async cts =>
        {
            WorkData work = new(workText ?? Strings.GenericLoading, cts)
            {
                Progress = initialProgress
            };

            infoBar?.Clear();

            using (progressBar.Begin(work))
            {
                try
                {
                    await func?.Invoke(work, cts);
                }
                catch (Exception ex)
                {
                    if (infoBar != null)
                    {
                        infoBar.SetError(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        });
    }
}
