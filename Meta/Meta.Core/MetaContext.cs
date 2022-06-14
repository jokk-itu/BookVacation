namespace Meta;

public class MetaContext
{
    public Guid CorrelationId { get; set; }  = Guid.NewGuid();

    public Guid RequestId { get; set; } = Guid.NewGuid();
}