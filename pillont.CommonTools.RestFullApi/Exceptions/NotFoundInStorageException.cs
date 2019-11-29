using System;
using System.Runtime.Serialization;

namespace pillont.CommonTools.RestFullApi.Exceptions
{
    [Serializable]
    public class NotFoundInStorageException : APIException
    {
        public override object ErrorBody => Message;

        public override int StatusCode => 404;

        public NotFoundInStorageException()
        { }

        public NotFoundInStorageException(string message)
            : base(message)
        { }

        public NotFoundInStorageException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public NotFoundInStorageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}