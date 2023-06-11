namespace MarkForth.Html;

internal sealed class HeadElementWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly HtmlContext _context;
    private readonly HtmlElementWriter _htmlElementWriter;

    internal HeadElementWriter(
        HtmlContext context,
        HtmlElementWriter htmlElementWriter
    ) : base(context, htmlElementWriter)
    {
        _context = context;
        _htmlElementWriter = htmlElementWriter;
    }

    #endregion Dependency Injection.

    internal HtmlElementWriter WriteHeadEnd()
    {
        WriteElementEndLine(ElementNames.Head);
        return _htmlElementWriter;
    }

    internal StyleElementWriter<HeadElementWriter> WriteStyleStart()
    {
        WriteElementStartLine(ElementNames.Style);
        return new(_context, this);
    }

    internal ScriptElementWriter WriteScriptStart()
    {
        WriteElementStartLine(ElementNames.Script);
        return new(_context, this);
    }

    internal TemplateElementWriter WriteTemplateStart(string componentName)
    {
        WriteElementStartLine
        (
            ElementNames.Template,
            new Attribute(AttributeNames.Id, componentName)
        );

        return new(_context, this);
    }
}
