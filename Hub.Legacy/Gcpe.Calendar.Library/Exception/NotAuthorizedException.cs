using System;
using System.Runtime.Serialization;

namespace CorporateCalendar.Exception
{
    [Serializable]
    public class NotAuthorizedException : System.Exception, System.Runtime.Serialization.ISerializable
    {
        public NotAuthorizedException()
            : base() {
        }

        public NotAuthorizedException(string message)
            : base(message) {
        }

        public NotAuthorizedException(string message, System.Exception InnerException)
            : base(message, InnerException) {
        }

        protected NotAuthorizedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) {
        }
    }
}