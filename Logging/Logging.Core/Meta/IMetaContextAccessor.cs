namespace Logging.Meta;

public interface IMetaContextAccessor
{
    MetaContext? MetaContext { get; set; }
}