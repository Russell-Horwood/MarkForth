using System.Text;

namespace MarkForth.Extensions.IO;

public static class StreamExtensions
{
    private static readonly Encoding encoding = new UTF8Encoding(false, true);

    /// <summary>
    /// Reads the text from the the current stream and writes them to a text writer.
    /// Both streams positions are advanced by the number of bytes copied.
    /// </summary>
    /// <param name="stream">
    /// The stream to copy from.
    /// </param>
    /// <param name="destination">
    /// The <see cref="TextWriter"/> to which the contents of the current stream will be copied.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// <paramref name="stream"/> does not support reading.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <paramref name="stream"/> is non-null
    /// and has non-zero length
    /// and <paramref name="destination"/> was closed before this method was called.
    /// </exception>
    /// <exception cref="IOException">
    /// <paramref name="stream"/> is non-null
    /// and has non-zero length
    /// and <paramref name="destination"/> produces an I/O error.
    /// -or- An I/O occurs reading from <paramref name="stream"/>.
    /// </exception>
    internal static TTextWriter CopyTo<TTextWriter>(this Stream stream, TTextWriter destination)
        where TTextWriter : TextWriter
    {
        foreach (char @char in stream.ReadToEnd())
            destination.Write(@char);

        return destination;
    }

    /// <summary>
    /// Reads the specified <see cref="Stream"/>
    /// from it's current position to it's end
    /// and converts to a string assuming UTF8 encoding.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to read.</param>
    /// <returns>
    /// The <see langword="string"/> equivalent of the remainder of <paramref name="stream"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="stream"/> does not support reading.
    /// </exception>
    /// <exception cref="OutOfMemoryException">
    /// There is insufficient memory to allocate a buffer for the returned string.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurs.
    /// </exception>
    public static string ReadAsString(this Stream stream)
    {
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8, false, -1, true);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Reads characters from a UTF-8 encoded stream.
    /// </summary>
    /// <param name="stream">
    /// The <see cref="Stream"/> to read characters from.
    /// </param>
    /// <remarks>
    /// The stream is advanced by the number of bytes the constitute each character
    /// as each character is enumerated from the return value.
    /// </remarks>
    /// <returns>
    /// An <see cref="IEnumerable{char}"/> that can be used to read characters
    /// sequentially from <paramref name="stream"/>.
    /// </returns>
    /// <exception cref="IOException">
    /// An I/O error occurs.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The current stream does not support reading.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// <paramref name="stream"/> was closed.
    /// </exception>
    internal static IEnumerable<char> ReadToEnd(this Stream stream)
    {
        byte[] buffer = new byte[4];
        int remainingByteCount;

        while (true)
        {
            if (stream.Read(buffer, 0, 1) == 0)
                yield break;

            if (buffer[0] <= 0x7F)
                yield return encoding.GetChars(buffer, 0, 1)[0];

            remainingByteCount =
                ((buffer[0] & 240) == 240) ? 3 : (
                ((buffer[0] & 224) == 224) ? 2 : (
                ((buffer[0] & 192) == 192) ? 1 : 0
            ));

            if (remainingByteCount != 0)
            {
                stream.Read(buffer, 1, remainingByteCount);
                yield return encoding.GetChars(buffer, 0, remainingByteCount + 1)[0];
            }
        }
    }
}
