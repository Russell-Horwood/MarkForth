namespace MarkForth.Html;

internal sealed class BodyElementWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly HtmlElementWriter _htmlElementWriter;

    internal BodyElementWriter(HtmlContext context, HtmlElementWriter htmlElementWriter)
        : base(context, htmlElementWriter)
    {
        _htmlElementWriter = htmlElementWriter;
    }

    #endregion Dependency Injection.

    internal HtmlElementWriter CloseBodyElement()
    {
        WriteElementEndLine(ElementNames.Body);
        return _htmlElementWriter;
    }
}
