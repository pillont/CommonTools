using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using pillont.CommonTools.Core.AspNetCore.Core.Exceptions;
using pillont.CommonTools.RestFullApi.Logic;

namespace pillont.CommonTools.RestFullApi.Controllers
{
    /// <summary>
    /// base class to generate RESTFull controller
    /// </summary>
    /// <typeparam name="TModel">type of the model send in the controller functions</typeparam>
    /// <typeparam name="TFilter">type of the filter used in the controller functions</typeparam>
    /// <typeparam name="TId">type of the id used in the model and entity</typeparam>
    /// <example>
    /// COMPLET TEMPLATE :
    /// |ctrl| + H => 'Feat'
    ///
    /// [RoutePrefix("api/Feats")]
    ///    public class FeatController : BaseRestFullController<FeatModel, FeatFilter, int>
    ///    {
    ///        public FeatController(FeatManager FeatManager, FeatValidator FeatValidator)
    ///            : base(FeatManager, FeatValidator)
    ///        { }
    ///
    ///        [HttpPost]
    ///        [Route("")]
    ///        public new Task<IHttpActionResult> CreateAsync([FromBody] FeatModel model)
    ///        {
    ///            return base.CreateAsync(model);
    ///        }
    ///
    ///        [HttpPost]
    ///        [Route("all")]
    ///        public new Task<IHttpActionResult> CreateAllAsync([FromBody] IEnumerable<FeatModel> model)
    ///        {
    ///            return base.CreateAllAsync(model);
    ///        }
    ///
    ///        [HttpGet]
    ///        [Route("{id}")]
    ///        public new Task<IHttpActionResult> GetAsync(int id)
    ///        {
    ///            return base.GetAsync(id);
    ///        }
    ///
    ///        [HttpGet]
    ///        [Route("")]
    ///        public new Task<IHttpActionResult> GetAllAsync([FromUri]FeatFilter filter)
    ///        {
    ///            return base.GetAllAsync(filter);
    ///        }
    ///
    ///        [HttpPut]
    ///        [Route("")]
    ///        public new Task<IHttpActionResult> UpdateAsync([FromBody] FeatModel model)
    ///        {
    ///            return base.UpdateAsync(model);
    ///        }
    ///
    ///        [HttpPut]
    ///        [Route("all")]
    ///        public new Task<IHttpActionResult> UpdateAllAsync([FromBody] IEnumerable<FeatModel> model)
    ///        {
    ///            return base.UpdateAllAsync(model);
    ///        }
    ///
    ///        [HttpDelete]
    ///        [Route("")]
    ///        protected new Task<IHttpActionResult> DeleteAsync(int id)
    ///        {
    ///            return base.DeleteAsync(id);
    ///        }
    ///    }
    /// </example>
    public class BaseRestFullController<TModel, TFilter, TId> : ApiController
    {
        /// <summary>
        /// manager to change and read values in storage
        /// </summary>
        internal ICrudManager<TModel, TFilter, TId> StoreManager { get; }

        /// <summary>
        /// check if values send in the functions are valid
        /// </summary>
        internal IValidator<TModel, TFilter, TId> Validator { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public BaseRestFullController(ICrudManager<TModel, TFilter, TId> manager, IValidator<TModel, TFilter, TId> validator)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            StoreManager = manager;

            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            Validator = validator;
        }

        /// <summary>
        /// create entity in storage
        /// </summary>
        /// <param name="model">value to populate new entity</param>
        /// <returns>new model created</returns>
        /// <example>
        ///
        ///         [HttpPost]
        ///         [Route("")]
        ///         public new Task<IHttpActionResult> CreateAsync([FromBody] TModel model)
        ///         {
        ///             return base.CreateAsync(model);
        ///         }
        ///
        /// </example>
        protected virtual async Task<IHttpActionResult> CreateAsync([FromBody] TModel model)
        {
            Validator.CheckForCreation(model);
            if (Validator.isFailed)
                return BadRequest(Validator.Message);

            var result = await StoreManager.CreateAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// create entity in storage
        /// </summary>
        /// <param name="model">value to populate new entity</param>
        /// <returns>new model created</returns>
        /// <example>
        ///
        ///         [HttpPost]
        ///         [Route("all")]
        ///         public new Task<IHttpActionResult> CreateAllAsync([FromBody] IEnumerable<FEATModel> model)
        ///         {
        ///             return base.CreateAllAsync(model);
        ///         }
        ///
        /// </example>
        protected virtual async Task<IHttpActionResult> CreateAllAsync([FromBody] IEnumerable<TModel> allModels)
        {
            Validator.CheckForMultiCreation(allModels);
            if (Validator.isFailed)
                return BadRequest(Validator.Message);

            var result = await StoreManager.CreateAllAsync(allModels);
            return Ok(result);
        }

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
        /// <example>
        ///
        ///        [HttpPut]
        ///        [Route("")]
        ///        public new Task<IHttpActionResult> UpdateAsync([FromBody] TModel model)
        ///        {
        ///            return base.UpdateAsync(model);
        ///        }
        ///
        ///</ example>

        protected virtual async Task<IHttpActionResult> UpdateAsync([FromBody] TModel model)
        {
            Validator.CheckForUpdate(model);
            if (Validator.isFailed)
                return BadRequest(Validator.Message);

            try
            {
                var result = await StoreManager.UpdateAsync(model);
                return Ok(result);
            }
            catch (NotFoundInStorageException)
            {
                return NotFound();
            }
        }

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
        /// <example>
        ///         [HttpPut]
        ///
        ///         [Route("all")]
        ///         public new Task<IHttpActionResult> UpdateAsync([FromBody] IEnumerable<TModel> model)
        ///         {
        ///             return base.UpdateAllAsync(model);
        ///         }
        ///
        /// </example>
        protected virtual async Task<IHttpActionResult> UpdateAllAsync([FromBody] IEnumerable<TModel> model)
        {
            Validator.CheckForMultiUpdate(model);
            if (Validator.isFailed)
                return BadRequest(Validator.Message);

            try
            {
                var result = await StoreManager.UpdateAllAsync(model);
                return Ok(result);
            }
            catch (NotFoundInStorageException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// get all entity associated to the filter
        /// </summary>
        /// <param name="filter">
        /// if null, no filter applied
        /// </param>
        /// <example>
        /// for controller with route : GET : "Feat"
        /// for filter :
        /// {
        ///     int prop1 {get;set;}
        ///     int[] prop2 {get;set;}
        /// }
        ///
        /// /////////// without filter /////////////////
        /// url : /Feat
        /// will have as filter
        /// {
        ///    prop1= 0,
        ///    prop2= null,
        /// }
        ///
        /// /////////// with filter /////////////////
        /// url : GET : /Feat?prop1=12&prop2=24&prop2=48
        /// will have as filter
        /// {
        ///    prop1= 12
        ///    prop2= [24,48]
        ///
        ///
        ///       [HttpGet]
        ///       [Route("")]
        ///       public new Task<IHttpActionResult> GetAllAsync(TFilter filter)
        ///       {
        ///           return base.GetAllAsync(filter);
        ///       }
        ///
        /// </example>
        protected virtual async Task<IHttpActionResult> GetAllAsync([FromUri]TFilter filter)
        {
            Validator.CheckFilter(filter);
            if (Validator.isFailed)
                return BadRequest(Validator.Message);

            var result = await StoreManager.GetAllAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// get model associated to the entity with same ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <example>
        /// for controller with route : "Feat"
        /// url : GET : /Feat/12
        /// will have as id : 12
        ///
        ///        [HttpGet]
        ///        [Route("{id}")]
        ///        public new Task<IHttpActionResult> GetAsync(TId id)
        ///        {
        ///            return base.GetAsync(id);
        ///        }
        ///
        /// </example>
        protected virtual async Task<IHttpActionResult> GetAsync(TId id)
        {
            Validator.CheckId(id);
            if (Validator.isFailed)
                return BadRequest(Validator.Message);

            try
            {
                var result = await StoreManager.GetByIdAsync(id);
                return Ok(result);
            }
            catch (NotFoundInStorageException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// delete entity with same id
        /// </summary>
        /// <example>
        /// for controller with route : "Feat"
        /// url : DELETE : /Feat/12
        /// will have as id : 12
        ///
        ///         [HttpDelete]
        ///         [Route("")]
        ///         public new Task<IHttpActionResult> DeleteAsync(TId id)
        ///         {
        ///             return base.DeleteAsync(id);
        ///         }
        ///
        /// </example>
        protected virtual async Task<IHttpActionResult> DeleteAsync(TId id)
        {
            Validator.CheckId(id);
            if (Validator.isFailed)
                return BadRequest(Validator.Message);

            try
            {
                await StoreManager.DeleteAsync(id);
                // NOTE : on doit retourner "" pour eviter une erreur d'ASP...
                // EXCEPTION : ArgumentOutOfRangeException
                return Ok(String.Empty);
            }
            catch (NotFoundInStorageException)
            {
                return NotFound();
            }
        }
    }
}