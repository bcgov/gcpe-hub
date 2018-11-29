using System;
using System.Runtime.Serialization;

namespace CorporateCalendar.Exception
{
    [Serializable]
    public class SecurityRoleNotFoundException : System.Exception, System.Runtime.Serialization.ISerializable
    {
        public SecurityRoleNotFoundException()
            : base() {
        }

        public SecurityRoleNotFoundException(string message)
            : base(message) {
        }

        public SecurityRoleNotFoundException(string message, System.Exception InnerException)
            : base(message, InnerException) {
        }

        protected SecurityRoleNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) {
        }
    }
}