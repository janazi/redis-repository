using System;

namespace Jnz.RedisRepository
{
    public class KeyLockedException : Exception
    {
        public new string Message { get; private set; }
        public KeyLockedException(string message)
            : base(message)
        {
            Message = message;
        }
    }
}
