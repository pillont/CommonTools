using System;
using System.Threading;
using System.Threading.Tasks;

namespace pillont.CommonTools.Core.Parallel;

/// <summary>
/// extension of semaphore to avoid not readable try/finally pattern
///
/// with async func, use semaphore, never use "lock" statement :
/// SOURCE : https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
/// </summary>
public static class SemaphoreExtension
{
    public static bool TryWaitFor<T>(
        this SemaphoreSlim semaphore,
        out T result,
        Func<CancellationToken, T> func,
        int millisecondsTimeout = -1,
        CancellationToken? cancellationToken = null)
    {
        T res = default;
        var success = WaitFor(
            semaphore,
            (token) => res = func(token),
            millisecondsTimeout,
            cancellationToken
        );

        result = res;
        return success;
    }

    /// <summary>
    /// apply action between single secure wait/release
    /// </summary>
    /// <param name="semaphore">semaphore to apply wait and release</param>
    /// <param name="action">action to apply between single wait/release</param>
    /// <example>
    /// m_Semaphore.WaitFor(Action, timeOut)
    /// ===
    /// try { Wait / Action } finally { Release(1) }
    /// </example>
    /// <seealso cref="SemaphoreSlim.Wait(int)"/>
    public static bool WaitFor(
        this SemaphoreSlim semaphore,
        Action<CancellationToken> action,
        int millisecondsTimeout = -1,
        CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? CancellationToken.None;

        var task = semaphore.WaitForAsync(action, millisecondsTimeout, token);
        task.Wait(token);

        return task.Result;
    }

    /// <summary>
    /// apply action between single secure wait/release in async process
    /// </summary>
    /// <param name="semaphore">semaphore to apply wait and release</param>
    /// <param name="action">action to apply between single wait/release</param>
    /// <example>
    /// m_Semaphore.WaitForAsync(Action, timeOut)
    /// ===
    /// try { Wait / Action } finally { Release(1) }
    /// </example>
    /// <seealso cref="SemaphoreSlim.WaitAsync(int)"/>
    public static async Task<bool> WaitForAsync(
        this SemaphoreSlim semaphore,
        Action<CancellationToken> action,
        int millisecondsTimeout = -1,
        CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? CancellationToken.None;

        var res = await semaphore.WaitAsync(millisecondsTimeout, cancellationToken: token);
        if (!res)
        {
            return false;
        }

        try
        {
            action?.Invoke(token);
        }
        finally
        {
            _ = semaphore.Release(1);
        }

        return true;
    }

    /// <summary>
    /// apply async action between single secure wait/release in async process
    /// </summary>
    /// <param name="semaphore">semaphore to apply wait and release</param>
    /// <param name="func">action to apply between single wait/release</param>
    /// <example>
    /// m_Semaphore.WaitForAsync(Action, timeOut)
    /// ===
    /// try { Wait / Action } finally { Release(1) }
    /// </example>
    /// <seealso cref="SemaphoreSlim.WaitAsync(int)"/>
    public static async Task<bool> WaitForAsync(
        this SemaphoreSlim semaphore,
        Func<CancellationToken, Task> func,
        int millisecondsTimeout = -1,
        CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? CancellationToken.None;
        var res = await semaphore.WaitAsync(millisecondsTimeout, cancellationToken: token);
        if (!res)
        {
            return false;
        }

        try
        {
            await func?.Invoke(token);
        }
        finally
        {
            _ = semaphore.Release(1);
        }

        return true;
    }
}