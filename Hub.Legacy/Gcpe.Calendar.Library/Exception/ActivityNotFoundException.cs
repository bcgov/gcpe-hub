using System;
using System.Runtime.Serialization;

namespace CorporateCalendar.Exception
{
    [Serializable]
    public class ActivityNotFoundException : System.Exception, System.Runtime.Serialization.ISerializable
    {
        public ActivityNotFoundException()
            : base() {
        }

        public ActivityNotFoundException(string message)
            : base(message) {
        }

        public ActivityNotFoundException(string message, System.Exception InnerException)
            : base(message, InnerException) {
        }

        protected ActivityNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) {
        }
    }
}
