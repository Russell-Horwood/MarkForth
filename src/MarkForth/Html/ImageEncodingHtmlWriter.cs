using MarkForth.Extensions.IO;
using MarkForth.IO;
using System.Security.Cryptography;

namespace MarkForth.Html;

internal sealed class ImageEncodingHtmlWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly ImgHtmlContext _context;
    private readonly IDirectory _directory;
    private readonly IFile _file;

    public ImageEncodingHtmlWriter(
        ImgHtmlContext context,
        IDirectory directory,
        IFile file,
        TextWriter innerTextWriter
    ) : base(context, innerTextWriter)
    {
        _context = context;
        _directory = directory;
        _file = file;
    }

    #endregion Dependency Injection.

    #region Write.

    private static readonly ToBase64Transform _toBase64Transform = new();
    private readonly List<char> _srcBuffer = new();
    private readonly byte[] base64Buffer = new byte[1024];

    protected override void WriteHtml(char value)
    {
        if (_context.IsInImgSrc)
        {
            _srcBuffer.Add(value);
        }
        else if (_srcBuffer.Any())
        {
            string src = String.Concat(_srcBuffer);

            if (new Uri(src).IsFile)
            {
                Write("data:image/png;base64, ");
                using Stream inputFile = _file.OpenRead(src);
                using CryptoStream base64Stream = new CryptoStream(inputFile, _toBase64Transform, CryptoStreamMode.Read);
                Write(base64Stream.ReadToEnd());
                Write("\" ");
            }

            _srcBuffer.Clear();
        }
        else
        {
            base.WriteHtml(value);
        }
    }

    #endregion Write.

}
