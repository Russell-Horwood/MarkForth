using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace MarkForth.Html;

internal sealed class HtmlDocumentWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly HtmlContext _context;

    public HtmlDocumentWriter(
        HtmlContext context,
        TextWriter innerWriter
    ) : base(context, innerWriter)
    {
        _context = context;
    }

    #endregion Dependency Injection.

    #region Create.

    internal static HtmlDocumentWriter Create(IServiceProvider serviceProvider, Stream stream)
    {
        return ActivatorUtilities.CreateInstance<HtmlDocumentWriter>
        (
            serviceProvider,
            ActivatorUtilities.CreateInstance<IndentingHtmlWriter>
            (
                serviceProvider,
                ActivatorUtilities.CreateInstance<ImageEncodingHtmlWriter>
                (
                    serviceProvider,
                    new StreamWriter(stream, Encoding.UTF8, -1, true)
                )
            )
       );
    }

    #endregion Create.

    internal HtmlElementWriter WriteHtmlStart()
    {
        WriteElementStartLine(ElementNames.Html);
        return new(_context, this);
    }
}
