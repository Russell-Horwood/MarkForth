namespace MarkForth.Html;

internal sealed class HtmlElementWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly HtmlContext _context;
    private readonly HtmlDocumentWriter _htmlDocumentWriter;

    internal HtmlElementWriter(
        HtmlContext context,
        HtmlDocumentWriter htmlDocumentWriter
    ) : base(context, htmlDocumentWriter)
    {
        _context = context;
        _htmlDocumentWriter = htmlDocumentWriter;
    }

    #endregion Dependency Injection.

    internal BodyElementWriter WriteBodyStart()
    {
        WriteElementStartLine(ElementNames.Body);
        return new(_context, this);
    }

    internal HeadElementWriter WriteHeadStart()
    {
        WriteElementStartLine(ElementNames.Head);
        return new(_context, this);
    }

    internal HtmlDocumentWriter WriteHtmlEnd()
    {
        WriteElementEndLine(ElementNames.Html);
        WriteLine();
        return _htmlDocumentWriter;
    }
}
