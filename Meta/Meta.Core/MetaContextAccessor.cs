namespace Meta;

public class MetaContextAccessor : IMetaContextAccessor
{
    private static readonly AsyncLocal<MetaContextHolder> MetaContextCurrent = new();

    public MetaContext? MetaContext
    {
        get => MetaContextCurrent.Value?.Context;
        set
        {
            var holder = MetaContextCurrent.Value;
            if (holder != null) holder.Context = null;

            if (value != null) MetaContextCurrent.Value = new MetaContextHolder { Context = value };
        }
    }

    private sealed class MetaContextHolder
    {
        public MetaContext? Context;
    }
}