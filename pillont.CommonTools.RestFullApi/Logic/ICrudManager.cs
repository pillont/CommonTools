using System.Collections.Generic;
using System.Threading.Tasks;

namespace pillont.CommonTools.RestFullApi.Logic
{
    /// <summary>
    /// base crud manager to manage storage
    /// </summary>
    /// <typeparam name="TEntity">type of the entity in DB</typeparam>
    /// <typeparam name="TModel">type of the model send by the Controller</typeparam>
    /// <typeparam name="TFilter">type of filter send by the Controller</typeparam>
    /// <typeparam name="TId">type of the entity id</typeparam>
    public interface ICrudManager<TModel, TFilter, TId>
    {
        /// <summary>
        /// create entity in DB
        /// </summary>
        /// <param name="model">value to populate new entity</param>
        /// <returns>new model created</returns>
        Task<TModel> CreateAsync(TModel model, bool saveChanges = true);

        /// <summary>
        /// create multi entity in DB
        /// </summary>
        /// <param name="model">value to populate new entities</param>
        /// <returns>new models created</returns>
        Task<IEnumerable<TModel>> CreateAllAsync(IEnumerable<TModel> allModels, bool saveChanges = true);

        /// <summary>
        /// delete entity with same id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(TId id);

        /// <summary>
        /// get all value associated to the filter
        /// </summary>
        /// <param name="filter">
        /// if null, no filter applied
        /// </param>
        Task<IEnumerable<TModel>> GetAllAsync(TFilter filter = default(TFilter));

        /// <summary>
        /// get model associated to the entity with same ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TModel> GetByIdAsync(TId id);

        /// <summary>
        /// update entity in DB with wanted value
        /// </summary>
        /// <param name="model">
        /// value to update entity
        /// must have ID
        /// </param>
        /// <returns>
        /// updated value
        /// </returns>
        Task<TModel> UpdateAsync(TModel model, bool saveChanges = true);

        /// <summary>
        /// update multi entities in DB with wanted values
        /// </summary>
        /// <param name="model">
        /// values to update entity
        /// must have ID
        /// </param>
        /// <returns>
        /// updated values
        /// </returns>
        Task<IEnumerable<TModel>> UpdateAllAsync(IEnumerable<TModel> model, bool saveChanges = true);
    }
}