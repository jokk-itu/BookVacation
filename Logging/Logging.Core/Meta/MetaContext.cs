namespace Logging.Meta;

public class MetaContext
{
    public string RequestId { get; set; }  = Guid.NewGuid().ToString();

    public string CorrelationId { get; set; }  = Guid.NewGuid().ToString();
}