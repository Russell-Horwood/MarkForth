using System.Text;

namespace MarkForth.IO;

internal abstract class DelegatingTextWriter : TextWriter
{

    #region Dependency Injection.

    private readonly TextWriter _innerWriter;

    protected DelegatingTextWriter(TextWriter innerWriter)
    {
        _innerWriter = innerWriter;
    }

    #endregion Dependency Injection.

    #region TextWriter.

    public override Encoding Encoding => _innerWriter.Encoding;

    public override void Close()
    {
        _innerWriter.Close();
    }

    public override void Flush()
    {
        _innerWriter.Flush();
    }

    public override void Write(char value)
    {
        _innerWriter.Write(value);
    }

    protected void Write(IEnumerable<char> chars)
    {
        foreach (char @char in chars)
            _innerWriter.Write(@char);
    }

    #region IDisposable.

    private bool _disposed;

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _innerWriter?.Dispose();
            }

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    #endregion IDisposable.

    #endregion TextWriter.

}
