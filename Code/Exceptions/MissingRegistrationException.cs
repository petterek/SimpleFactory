using System;
using System.Runtime.Serialization;

namespace SimpleFactory.Exceptions
{
    [Serializable]

    public class UnableToConstructExcpetion : Exception
    {
        
    }

    public class MissingRegistrationException : Exception
    {
        private Type _type;

        public MissingRegistrationException()
        {
        }

        public MissingRegistrationException(Type type):this(type.ToString())
        {
            _type = type;
        }

        public MissingRegistrationException(string message) : base(message)
        {
        }

        public MissingRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}