using System.Runtime.Serialization;

namespace SkillHaven.Shared.Exceptions;

public class UserVerifyException : Exception
{
    public UserVerifyException()
    {
    }

    public UserVerifyException(string message) : base(message)
    {
    }

    public UserVerifyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected UserVerifyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
