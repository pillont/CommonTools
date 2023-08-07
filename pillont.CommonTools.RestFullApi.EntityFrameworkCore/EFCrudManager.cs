using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pillont.CommonTools.Core.AspNetCore.Core.Exceptions;
using pillont.CommonTools.RestFullApi.Logic;

namespace pillont.CommonTools.RestFullApi.EntityFrameworkCore
{
    /// <summary>
    /// base class to manage entity with EF
    /// </summary>
    /// <typeparam name="TEntity">type of the entity in DB</typeparam>
    /// <typeparam name="TModel">type of the model send by the Controller</typeparam>
    /// <typeparam name="TFilter">type of filter send by the Controller</typeparam>
    /// <typeparam name="TId">type of the entity id</typeparam>
    public abstract class EFCrudManager<TEntity, TModel, TFilter, TId> : ICrudManager<TModel, TFilter, TId> where TEntity : class
    {
        /// <summary>
        /// EF context to manage change
        /// </summary>
        public DbContext Context { get; }

        /// <summary>
        /// list to apply request
        /// </summary>
        public DbSet<TEntity> DbSet { get; }

        public EFCrudManager(DbContext context, DbSet<TEntity> dbSet)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        }

        /// <summary>
        /// class to check if entity have wanted ID
        /// </summary>
        protected abstract Expression<Func<TEntity, bool>> HaveSameId(TId id);

        /// <summary>
        /// check if value is associated to the filter
        /// </summary>
        /// <returns>
        /// true if entity associated to filter
        /// else false
        /// </returns>
        protected abstract Expression<Func<TEntity, bool>> GetFilterPredicate(TFilter filter);

        /// <summary>
        /// populate entity with model values
        /// keep id of the model if not null
        /// </summary>
        /// <param name="model">value to populate model</param>
        /// <param name="entity">if null, create new value and result must have empty ID</param>
        protected abstract TEntity ToEntity(TModel model, TEntity entity = null);

        /// <summary>
        /// check if entity have same id than model
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected abstract Expression<Func<TEntity, bool>> HaveSameId(TModel model);

        /// <summary>
        /// populate new model with entity value
        /// </summary>
        protected abstract TModel ToModel(TEntity entity);

        /// <summary>
        /// if return not null, used in GetAll query to avoid browse multi time the result
        /// </summary>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, TModel>> ToModelPredicate()
        {
            return null;
        }

        /// <inherit>
        public virtual async Task<TModel> CreateAsync(TModel model, bool saveChanges = true)
        {
            TEntity entity = ToEntity(model);
            DbSet.Add(entity);
            if (saveChanges)
                await Context.SaveChangesAsync();
            return ToModel(entity);
        }

        /// <inherit>
        public virtual async Task<IEnumerable<TModel>> CreateAllAsync(IEnumerable<TModel> allModels, bool saveChanges = true)
        {
            var result = allModels.Select(model => ToEntity(model));
            DbSet.AddRange(result);

            if (saveChanges)
                await Context.SaveChangesAsync();

            return result.Select(entity => ToModel(entity))
                         .ToList();
        }

        /// <inherit>
        public virtual async Task<TModel> UpdateAsync(TModel model, bool saveChanges = true)
        {
            var toUpdate = await DbSet.FirstOrDefaultAsync(HaveSameId(model));
            if (toUpdate == null)
                throw new NotFoundInStorageException("item with same id not found");

            var result = ToEntity(model, toUpdate);

            if (saveChanges)
                await Context.SaveChangesAsync();

            return ToModel(result);
        }

        /// <inherit>
        public virtual async Task<IEnumerable<TModel>> UpdateAllAsync(IEnumerable<TModel> allModels, bool saveChanges = true)
        {
            var allUpdated = new List<TModel>();
            foreach (var model in allModels)
            {
                var updatedModel = await UpdateAsync(model, saveChanges: false);
                allUpdated.Add(updatedModel);
            }

            if (saveChanges)
                await Context.SaveChangesAsync();

            return allUpdated;
        }

        /// <inherit>
        public virtual async Task<IEnumerable<TModel>> GetAllAsync(TFilter filter = default(TFilter))
        {
            Expression<Func<TEntity, bool>> filterPredicate = filter != null
                                                        ? GetFilterPredicate(filter)
                                                        : e => true;

            var query = DbSet.Where(filterPredicate)
                                .AsNoTracking();

            // NOTE : if predicate, use if to avoid browse 2 time the list
            var predicate = ToModelPredicate();
            if (predicate != null)
                return await query.Select(predicate)
                                  .ToListAsync();

            // NOTE : if no predicate : collect all and select after
            var allEntities = await query.ToListAsync();
            var allModels = allEntities.Select(e => ToModel(e))
                                       .ToList();
            return allModels;
        }

        /// <inherit>
        public virtual async Task<TModel> GetByIdAsync(TId id)
        {
            TEntity entity = await DbSet.AsNoTracking()
                                        .FirstOrDefaultAsync(HaveSameId(id));
            if (entity == null)
                throw new NotFoundInStorageException("entity with same id not found");

            return ToModel(entity);
        }

        /// <inherit>
        public virtual async Task DeleteAsync(TId id)
        {
            await DbSet.Where(HaveSameId(id))
                       .DeleteFromQueryAsync();
        }
    }
}