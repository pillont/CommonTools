using System.Collections.Generic;

namespace pillont.CommonTools.RestFullApi.Logic
{
    /// <summary>
    ///  check if model is valid
    /// </summary>
    /// <typeparam name="TEntity">type of the entity in DB</typeparam>
    /// <typeparam name="TModel">type of the model send by the Controller</typeparam>
    /// <typeparam name="TFilter">type of filter send by the Controller</typeparam>
    /// <typeparam name="TId">type of the entity id</typeparam>
    public interface IValidator<TModel, TFilter, TId>
    {
        bool isFailed { get; }
        string Message { get; }

        /// <summary>
        /// HERE : add model check during creation
        /// populate message if not valid
        /// </summary>
        void CheckForCreation(TModel model);

        /// <summary>
        /// HERE : add model check during update
        /// populate message if not valid
        /// </summary>
        void CheckForUpdate(TModel model);

        /// <summary>
        /// HERE : Check filter
        /// populate message if not valid
        /// </summary>
        void CheckFilter(TFilter model);

        /// <summary>
        /// HERE : add id check
        /// populate message if not valid
        /// </summary>
        void CheckId(TId id);

        void CheckForMultiCreation(IEnumerable<TModel> allModels);

        void CheckForMultiUpdate(IEnumerable<TModel> model);
    }
}