using System;
using System.Runtime.Serialization;

namespace CorporateCalendar.Exception
{
    [Serializable]
    public class InvalidUserAccountException : System.Exception, System.Runtime.Serialization.ISerializable
    {
        public InvalidUserAccountException()
            : base() {
        }

        public InvalidUserAccountException(string message)
            : base(message) {
        }

        public InvalidUserAccountException(string message, System.Exception InnerException)
            : base(message, InnerException) {
        }

        protected InvalidUserAccountException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) {
        }
    }
}