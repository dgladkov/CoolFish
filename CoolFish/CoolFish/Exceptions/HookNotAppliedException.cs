using System;

namespace CoolFishNS.Exceptions
{
    /// <summary>
    ///     Exception that is thrown when a method is called while a Hook is not applied
    /// </summary>
    [Serializable]
    public class HookNotAppliedException : Exception
    {
        /// <inheritdoc />
        public HookNotAppliedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public HookNotAppliedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}