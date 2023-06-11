using System.Text;

namespace MarkForth.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Converts a <see langword="string"/> into a UTF8 encoded <see cref="MemoryStream"/>.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The <see cref="MemoryStream"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public static MemoryStream ToMemoryStream(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        MemoryStream memoryStream = new MemoryStream
        (
            Encoding.UTF8.GetBytes(value)
        );

        memoryStream.Position = 0;

        return memoryStream;
    }
}
