namespace Jnz.RedisRepository;

public class KeyLockedException : Exception
{
    public KeyLockedException(string message)
        : base(message) => Message = message;

    public new string Message { get; }

    public static KeyLockedException Throw(string message) => new(message);
}