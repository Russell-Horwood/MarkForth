namespace MarkForth.Extensions.IO;

internal static class TextReaderExtensions
{
    internal static TTextWriter CopyTo<TTextWriter>(this TextReader textReader, TTextWriter destination)
        where TTextWriter : TextWriter
    {
        char[] buffer = new char[1024];
        int readCount;
        do
        {
            readCount = textReader.Read(buffer, 0, buffer.Length);
            destination.Write(buffer, 0, readCount);
        }
        while (readCount != 0);

        return destination;
    }
}
