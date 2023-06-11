using MarkForth.IO;

namespace MarkForth.Html;

internal abstract class HtmlWriter : DelegatingTextWriter
{

    //TODO: Comments.

    #region Dependency Injection.

    private readonly HtmlContext _context;

    protected HtmlWriter(
        HtmlContext context,
        TextWriter innerWriter
    ) : base(innerWriter)
    {
        _context = context;
    }

    #endregion Dependency Injection.

    #region Write.

    public sealed override void Write(char value)
    {
        _context.Update(value);
        WriteHtml(value);
    }

    protected virtual void WriteHtml(char value)
    {
        base.Write(value);
    }

    #endregion Write.

    #region WriteElement.

    internal void WriteElementStartLine(string elementName, params Attribute[] attributes)
    {
        Write($"<{elementName}");

        foreach (Attribute attribute in attributes)
            Write($" {attribute.Name}=\"{attribute.Value}\"");

        WriteLine('>');
    }

    internal void WriteElementEndLine(string elementName)
    {
        WriteLine($"</{elementName}>");
    }

    #endregion WriteElement.

}
