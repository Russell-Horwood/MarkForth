namespace MarkForth.Html;

internal sealed class StyleElementWriter<TParentWriter>
    : HtmlWriter
    where TParentWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly TParentWriter _parentWriter;

    internal StyleElementWriter(
        HtmlContext context,
        TParentWriter parentWriter
    ) : base(context, parentWriter)
    {
        _parentWriter = parentWriter;
    }

    #endregion Dependency Injection.

    internal TParentWriter WriteStyleEnd()
    {
        WriteElementEndLine(ElementNames.Style);
        return _parentWriter;
    }
}
