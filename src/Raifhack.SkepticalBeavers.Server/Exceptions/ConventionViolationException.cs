using System;
using System.Runtime.Serialization;

namespace Raifhack.SkepticalBeavers.Server.Exceptions
{
    /// <inheritdoc />
    [Serializable]
    public sealed class ConventionViolationException : Exception
    {
        /// <inheritdoc />
        public ConventionViolationException() : base()
        {
        }

        /// <inheritdoc />
        public ConventionViolationException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ConventionViolationException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc />
        public ConventionViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}