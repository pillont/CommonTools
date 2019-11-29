using System;
using System.Collections.Generic;

namespace tpillon.CommonTools.Reflection.ReflectionCaches
{
    /// <summary>
    /// abstract class to make cache
    /// </summary>
    /// <typeparam name="TFilter">type of the object to collect value</typeparam>
    /// <typeparam name="TResult">type of result returned</typeparam>
    public abstract class ListReflectionCache<TFilter, TResult> : BaseReflectionCache<TFilter, IEnumerable<TResult>>
    {
        /// <summary>
        /// func to filter arguments result
        /// </summary>
        public Func<TResult, bool> IsValidResultCallBack { get; set; }

        /// <summary>
        /// filter results
        /// use <see cref="IsValidResultCallBack"/>
        /// if null : valid value
        /// </summary>
        protected virtual bool IsValidResult(TResult p_Argument)
        {
            return IsValidResultCallBack?.Invoke(p_Argument) ?? true;
        }
    }
}