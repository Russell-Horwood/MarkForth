namespace MarkForth.Html;

internal sealed class TemplateElementWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly HtmlContext _context;
    private readonly HeadElementWriter _headElementWriter;

    internal TemplateElementWriter(
        HtmlContext context,
         HeadElementWriter headElementWriter
    ) : base(context, headElementWriter)
    {
        _context = context;
        _headElementWriter = headElementWriter;
    }

    #endregion Dependency Injection.

    internal HeadElementWriter WriteTemplateEnd()
    {
        WriteElementEndLine(ElementNames.Template);
        return _headElementWriter;
    }

    internal StyleElementWriter<TemplateElementWriter> WriteStyleStart()
    {
        WriteElementStartLine(ElementNames.Style);
        return new(_context, this);
    }
}
