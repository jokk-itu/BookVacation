namespace Logging.Meta;

public class MetaContextAccessor : IMetaContextAccessor
{
    private static readonly AsyncLocal<MetaContextHolder> _metaContextCurrent = new();
    
    public MetaContext? MetaContext
    {
        get => _metaContextCurrent.Value?.Context;
        set
        {
            var holder = _metaContextCurrent.Value;
            if (holder != null)
            {
                holder.Context = null;
            }

            if (value != null)
            {
                _metaContextCurrent.Value = new MetaContextHolder { Context = value };
            }
        }
    }

    private sealed class MetaContextHolder
    {
        public MetaContext? Context;
    }
}