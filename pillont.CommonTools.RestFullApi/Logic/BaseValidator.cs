using System;
using System.Collections.Generic;
using System.Linq;

namespace pillont.CommonTools.RestFullApi.Logic
{
    public class BaseValidator<TModel, TFilter, TId> : IValidator<TModel, TFilter, TId>
    {
        public bool isFailed => AllMessages.Any();

        string IValidator<TModel, TFilter, TId>.Message => string.Join(",\n", AllMessages);

        public IList<string> AllMessages { get; }

        public BaseValidator()
            : this(new List<string>())
        { }

        public BaseValidator(IList<string> allMessage)
        {
            AllMessages = allMessage ?? throw new ArgumentNullException(nameof(allMessage));
        }

        public virtual void CheckFilter(TFilter limit)
        {
            // HERE : Check filter
        }

        public virtual void CheckForCreation(TModel limit)
        {
            AllMessages.Clear();
            // HERE : add model check during creation
        }

        public virtual void CheckForUpdate(TModel limit)
        {
            AllMessages.Clear();

            // HERE : add model check during update
        }

        public virtual void CheckId(TId id)
        {
            // HERE : add id check
        }

        public virtual void CheckForMultiCreation(IEnumerable<TModel> allModels)
        {
            if (allModels == null
            || !allModels.Any())
                AllMessages.Add("body list must not be empty during creation");

            foreach (var model in allModels)
            {
                CheckForCreation(model);
            }
        }

        public virtual void CheckForMultiUpdate(IEnumerable<TModel> allModels)
        {
            if (allModels == null
            || !allModels.Any())
                AllMessages.Add("body list must not be be empty during update");

            foreach (var model in allModels)
            {
                CheckForUpdate(model);
            }
        }
    }
}