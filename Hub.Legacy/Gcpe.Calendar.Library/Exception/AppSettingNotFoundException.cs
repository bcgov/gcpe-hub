using System;
using System.Runtime.Serialization;

namespace CorporateCalendar.Exception
{
    [Serializable]
    public class AppSettingNotFoundException : ArgumentException, System.Runtime.Serialization.ISerializable
    {
        public AppSettingNotFoundException()
            : base() {
        }

        public AppSettingNotFoundException(string message)
            : base(message) {
        }

        public AppSettingNotFoundException(string message, System.Exception InnerException)
            : base(message, InnerException) {
        }

        protected AppSettingNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) {
        }
    }
}