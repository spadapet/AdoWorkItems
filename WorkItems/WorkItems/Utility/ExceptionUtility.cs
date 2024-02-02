using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkItems.Utility;

internal static class ExceptionUtility
{
    public static string FlattenMessages(this Exception exception) => string.Join(" --> ", exception.Flatten().Select(e => e.Message));

    public static IEnumerable<Exception> Flatten(this Exception exception)
    {
        if (exception == null)
        {
            yield break;
        }

        // The message for AggregateException is not useful
        if (exception is not AggregateException)
        {
            yield return exception;
        }

        foreach (Exception innerException in exception.InnerException.Flatten())
        {
            yield return innerException;
        }

        if (exception is AggregateException aggregateException)
        {
            foreach (Exception innerException in aggregateException.InnerExceptions)
            {
                foreach (Exception innerException2 in innerException.Flatten())
                {
                    yield return innerException2;
                }
            }
        }
    }

    /// <returns>true if there were no exceptions ignored</returns>
    public static bool IgnoreExceptions(Action action)
    {
        try
        {
            action();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
