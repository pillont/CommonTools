using System.Collections.Specialized;

namespace pillont.CommonTools.Core.AspNetCore.Core.Exceptions;

/// <summary>
/// use in API to return BadRequest 400
/// </summary>
[Serializable]
public class InconsistantBodyException : APIException
{
    public override int StatusCode => 400;

    /// <summary>
    /// inform body of request is inconsistant
    /// </summary>
    public InconsistantBodyException()
        : base()
    {
        ValidationErrors = new NameValueCollection();
    }

    /// <summary>
    /// collections of paramNam / message to inform errors
    /// </summary>
    /// <remarks>
    /// NameValueCollection better than dictionary because same key can be used multi time to inform multi error of same params
    /// </remarks>
    public NameValueCollection ValidationErrors { get; }

    public override object ErrorBody => ValidationErrors.OfType<string>()
                                                        .ToDictionary(key => key,
                                                                        key => ValidationErrors.GetValues(key));

    /// <summary>
    /// inform body of request is inconsistant
    /// </summary>
    /// <example>
    /// new InconsistanceBodyException("param1", "can not be less than 1")
    ///
    /// => response body :
    /// {
    ///     "param1": [
    ///         "can not be less than 1"
    ///     ]
    /// }
    /// </example>
    /// <param name="key">
    /// param name
    /// </param>
    /// <param name="message">
    /// errors message
    /// </param>
    public InconsistantBodyException(string key, string message)
        : this(new NameValueCollection() { { key, message } })
    { }

    /// <summary>
    /// inform body of request is inconsistant
    /// </summary>
    /// <example>
    /// new InconsistanceBodyException(new NameValueCollection()
    /// {
    ///     {"param1", "can not be less than 1"},
    ///     {"param1", "can not be greater than 5"},
    ///     {"param2", "can not be null"},
    /// }
    ///
    /// => response body :
    /// {
    ///     "param1": [
    ///         "can not be less than 1"
    ///         "can not be greater than 5"
    ///     ]
    ///     "param2": [
    ///         "can not be less than 1, can not be greater than 5"
    ///     ]
    /// }
    /// </example>
    /// <param name="keysToMessages">
    /// collection of pair<string,string> to have key and associated errors message
    /// </param>
    public InconsistantBodyException(NameValueCollection keysToMessages)
        : this()
    {
        ValidationErrors = keysToMessages;
    }

    /// <summary>
    /// inform body of request is inconsistant
    /// </summary>
    /// <example>
    /// new InconsistanceBodyException(new NameValueCollection()
    /// {
    ///     {"param1", "can not be less than 1"},
    ///     {"param2", "can not be null"},
    /// }
    ///
    /// => response body :
    /// {
    ///     "param1": [
    ///         "can not be less than 1"
    ///     ]
    ///     "param2": [
    ///         "can not be less than 1, can not be greater than 5"
    ///     ]
    /// }
    /// </example>
    /// <param name="keysToMessages">
    /// collection of pair<string,string> to have key and associated errors message
    /// </param>
    public InconsistantBodyException(IEnumerable<KeyValuePair<string, string>> keysToMessages)
        : this()
    {
        foreach (var pair in keysToMessages)
        {
            ValidationErrors.Add(pair.Key, pair.Value);
        }
    }
}