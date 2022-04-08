using System.Reflection;
using System.Runtime.Serialization;

namespace Logging;

[Serializable]
public class LoggingConfigurationException : Exception
{
    public LoggingConfigurationException(MemberInfo type, string value) : base($"Invalid {type.Name} set to {value}") {}

    protected LoggingConfigurationException(SerializationInfo serializationInfo, StreamingContext streamingContext) :
        base(serializationInfo, streamingContext)
    {
    }
}