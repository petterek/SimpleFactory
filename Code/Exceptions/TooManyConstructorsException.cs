using System;
using System.Runtime.Serialization;

namespace SimpleFactory.Exceptions
{
    [Serializable]
    internal class TooManyConstructorsException : Exception
    {
        public TooManyConstructorsException(Type theType) : base(theType.FullName) { }

        public TooManyConstructorsException(string message) : base(message) { }

        public TooManyConstructorsException(string message, Exception innerException) : base(message, innerException) { }

        protected TooManyConstructorsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
