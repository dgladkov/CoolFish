using System;

namespace CoolFishNS.Exceptions
{
    /// <summary>
    ///     Exception that is thrown when a method is called while a Hook is not applied
    /// </summary>
    [Serializable]
    public class CodeInjectionFailedException : Exception
    {
        /// <inheritdoc />
        public CodeInjectionFailedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public CodeInjectionFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}