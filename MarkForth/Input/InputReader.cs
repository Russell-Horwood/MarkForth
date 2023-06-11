using MarkForth.Html;
using Microsoft.Extensions.DependencyInjection;

namespace MarkForth.Input;

internal sealed class InputReader : StreamReader
{

    #region Dependency Injection.

    private readonly ImgHtmlContext _context;
    private readonly FileStream _inputStream;

    public InputReader(
        ImgHtmlContext context,
        FileStream inputStream
    ) : base(inputStream)
    {
        _context = context;
        _inputStream = inputStream;
    }

    #endregion Dependency Injection.

    internal static TextReader Create(IServiceProvider serviceProvider, Stream stream)
    {
        if (stream is FileStream fileStream)
            return ActivatorUtilities.CreateInstance<InputReader>(serviceProvider, fileStream);

        return new StreamReader(stream);
    }

    Queue<char> _buffer = new();

    public override int Peek()
    {
        readToBuffer();

        return _buffer.TryPeek(out char result)
            ? result
            : -1;
    }

    public override int Read(char[] buffer, int index, int count)
    {
        int result;

        for (int i = index, j = index + count; i < j; i++)
        {
            result = Read();
            if (result == -1)
                return i - index;

            buffer[i] = (char)result;
        }

        return count;
    }

    public override int Read()
    {
        readToBuffer();

        return _buffer.TryDequeue(out char result)
            ? result
            : -1;
    }

    private void readToBuffer()
    {
        if (_buffer.Count != 0)
            return;

        bufferNext();

        if (_context.IsInImgSrc)
        {
            while (_context.IsInImgSrc)
                bufferNext();

            string path = String.Concat(_buffer);

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine
                (
                    Path.GetDirectoryName(_inputStream.Name)!,
                    path
                );

                _buffer.Clear();
                foreach (char @char in path)
                    _buffer.Enqueue(@char);
            }
        }
    }

    private void bufferNext()
    {
        int nextInt = base.Read();
        if (nextInt == -1)
            return;

        char nextChar = (char)nextInt;
        _buffer.Enqueue(nextChar);
        _context.Update(nextChar);
    }
}
