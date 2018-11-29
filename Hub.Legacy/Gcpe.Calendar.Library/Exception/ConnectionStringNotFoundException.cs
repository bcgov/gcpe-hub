using System;
using System.Runtime.Serialization;

namespace CorporateCalendar.Exception
{
    [Serializable]
    public class ConnectionStringNotFoundException : System.Exception, System.Runtime.Serialization.ISerializable
    {
        public ConnectionStringNotFoundException()
            : base() {
        }

        public ConnectionStringNotFoundException(string message)
            : base(message) {
        }

        public ConnectionStringNotFoundException(string message, System.Exception InnerException)
            : base(message, InnerException) {
        }

        protected ConnectionStringNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) {
        }
    }
}
