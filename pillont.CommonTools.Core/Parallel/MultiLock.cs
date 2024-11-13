using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace pillont.CommonTools.Core.Parallel;

/// <summary>
/// container of lockers with associated key
/// used to be sure to have only one semaphores by specific keys
/// </summary>
/// <typeparam name="TKey">Typeof the key</typeparam>
/// <example>
///
/// const string totoKey = "totoKey";
/// const string titiKey = "titiKey";
///
/// // like dictionnary of lock :
/// // on first usage of key => create new semaphore
/// // on other usages of key => collect first created semaphore
/// static MultiLock<string> lockContainer = new MultiLock<string>(p_AccessCountByKey: 1);
///
///
/// public void TotoLogic()
/// {
///     // In first time, a new semaphore while be generate
///     // in other case, the same semaphore while be collect again to have thread safe action
///     var semToto = lockContainer.GetCurrentLock(totoKey);
///
///     semToto.WaitFor(()=>
///     {
///         // so this function is thread safe :
///         // thread can enter here only one by one because p_AccessCountByKey is 1
///         [...]
///     });
/// }
///
/// public void TitiLogic()
/// {
///     // In first time, a new semaphore while be generate
///     // in other case, the same semaphore while be collect again to have thread safe action
///     var semTiti = lockContainer.GetCurrentLock(titiKey);
///
///     semTiti.WaitFor(()=>
///     {
///         // so this function is thread safe :
///         // thread can enter here only one by one because p_AccessCountByKey is 1
///         [...]
///     });
/// }
/// </example>
public class MultiLock<TKey>
{
    /// <summary>
    ///
    /// </summary>
    public int AccessCountByKey { get; }

    /// <summary>
    /// Lock for internal dictionnary
    /// </summary>
    private SemaphoreSlim DictLock { get; }

    /// <summary>
    /// Locks for concurrent accesses
    /// </summary>
    /// <remarks>This dictionnary is not thread-safe ! Please use <see cref="DictLock"/> to lock it.</remarks>
    private IDictionary<TKey, SemaphoreSlim> Locks { get; }

    public MultiLock(int p_AccessCountByKey, IDictionary<TKey, SemaphoreSlim> p_Locks)
    {
        if (p_AccessCountByKey < 1)
            throw new ArgumentOutOfRangeException(nameof(p_AccessCountByKey), "Number of concurent accesses must be greater or equal to 1.");
        AccessCountByKey = p_AccessCountByKey;

        Locks = p_Locks ?? throw new ArgumentNullException(nameof(p_Locks), "You must provide an initialized lock dictionnary");

        // NOTE : only one access to the locks dictionnary
        DictLock = new SemaphoreSlim(1, 1);
    }

    public MultiLock(int p_AccessCountByKey)
           : this(p_AccessCountByKey, new ConcurrentDictionary<TKey, SemaphoreSlim>())
    { }

    /// <summary>
    /// Get the lock for a given key
    /// in first time : create lock
    /// in other times : collect first created lock
    /// </summary>
    /// <param name="p_Key">Key to watch for</param>
    /// <returns>Lock for the key</returns>
    public virtual SemaphoreSlim GetCurrentLock(TKey p_Key)
    {
        SemaphoreSlim v_Lock = default;

        // here lock dictionary
        var v_Success = DictLock.WaitFor((ct) =>
        {
            if (!Locks.ContainsKey(p_Key))
            {
                Locks[p_Key] = CreateNewLock();
            }

            v_Lock = Locks[p_Key];
        });

        return !v_Success ? throw new InvalidOperationException("concurrent action not possible") : v_Lock;
    }

    protected virtual SemaphoreSlim CreateNewLock()
    {
        return new SemaphoreSlim(AccessCountByKey, AccessCountByKey);
    }
}