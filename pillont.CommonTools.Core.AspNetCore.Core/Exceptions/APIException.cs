using System.Runtime.Serialization;

namespace pillont.CommonTools.Core.AspNetCore.Core.Exceptions;

/// <summary>
/// Base exception to return specific status code by the API
/// </summary>
/// <remarks>
/// this class is in common project to be use by API and validators
/// </remarks>
public abstract class APIException : Exception
{
    public abstract int StatusCode { get; }

    public abstract object ErrorBody { get; }

    public APIException()
    { }

    public APIException(string message) : base(message)
    { }

    public APIException(string message, Exception innerException) : base(message, innerException)
    { }

    protected APIException(SerializationInfo info, StreamingContext context) : base(info, context)
    { }
}