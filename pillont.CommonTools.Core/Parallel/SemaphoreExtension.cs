using System;
using System.Threading;
using System.Threading.Tasks;

namespace pillont.CommonTools.Core.Parallel
{
    /// <summary>
    /// extension of semaphore to avoid not readable try/finally pattern
    ///
    /// with async func, use semaphore, never use "lock" statement :
    /// SOURCE : https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
    /// </summary>
    public static class SemaphoreExtension
    {
        /// <summary>
        /// apply action between single secure wait/release
        /// </summary>
        /// <param name="p_Semaphore">semaphore to apply wait and release</param>
        /// <param name="p_Action">action to apply between single wait/release</param>
        /// <example>
        /// m_Semaphore.WaitFor(Action, timeOut)
        /// ===
        /// try { Wait / Action } finally { Release(1) }
        /// </example>
        /// <seealso cref="SemaphoreSlim.Wait(int)"/>
        public static bool WaitFor(this SemaphoreSlim p_Semaphore, Action p_Action, int p_MillisecondsTimeout = -1)
        {
            var v_Task = p_Semaphore.WaitForAsync(p_Action, p_MillisecondsTimeout);
            v_Task.Wait();

            return v_Task.Result;
        }

        /// <summary>
        /// apply action between single secure wait/release in async process
        /// </summary>
        /// <param name="p_Semaphore">semaphore to apply wait and release</param>
        /// <param name="p_Action">action to apply between single wait/release</param>
        /// <example>
        /// m_Semaphore.WaitForAsync(Action, timeOut)
        /// ===
        /// try { Wait / Action } finally { Release(1) }
        /// </example>
        /// <seealso cref="SemaphoreSlim.WaitAsync(int)"/>
        public static async Task<bool> WaitForAsync(this SemaphoreSlim p_Semaphore, Action p_Action, int p_MillisecondsTimeout = -1)
        {
            bool v_Res = await p_Semaphore.WaitAsync(p_MillisecondsTimeout);
            if (v_Res == false)
                return false;
            try
            {
                p_Action?.Invoke();
            }
            finally
            {
                p_Semaphore.Release(1);
            }

            return true;
        }

        /// <summary>
        /// apply async action between single secure wait/release in async process
        /// </summary>
        /// <param name="p_Semaphore">semaphore to apply wait and release</param>
        /// <param name="p_Action">action to apply between single wait/release</param>
        /// <example>
        /// m_Semaphore.WaitForAsync(Action, timeOut)
        /// ===
        /// try { Wait / Action } finally { Release(1) }
        /// </example>
        /// <seealso cref="SemaphoreSlim.WaitAsync(int)"/>
        public static async Task<bool> WaitForAsync(this SemaphoreSlim p_Semaphore, Func<Task> p_Action, int p_MillisecondsTimeout = -1)
        {
            bool v_Res = await p_Semaphore.WaitAsync(p_MillisecondsTimeout);
            if (v_Res == false)
                return false;
            try
            {
                await p_Action?.Invoke();
            }
            finally
            {
                p_Semaphore.Release(1);
            }

            return true;
        }
    }
}