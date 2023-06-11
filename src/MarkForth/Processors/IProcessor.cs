using MarkForth.Storage;
using System.Security;
using System.Xml;

namespace MarkForth.Processors;

public interface IProcessor
{
    /// <summary>
    /// Processes a MarkForth input stream,
    /// and writes the resultant HTML to the specified output stream.
    /// </summary>
    /// <param name="input">
    /// The MarkForth input stream.
    /// </param>
    /// <param name="output">
    /// The stream to which the HTML output.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="input"/> is <see langword="null" />.
    /// -or- <paramref name="output"/> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="input"/> does not support reading.
    /// -or- <paramref name="output"/> is not writable.
    /// </exception>
    /// <exception cref="XmlException">
    /// The content of <paramref name="input"/> is not valid XML.
    /// </exception>
    /// <exception cref="SecurityException">
    /// The caller does not have sufficient permissions to access the source location of <paramref name="input"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// More than one usable Scaffolds were found.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurs while writing to <paramref name="output"/>.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error occurred while interacting with MarkForth's storage system.
    /// </exception>
    void Process(Stream input, Stream output);
}
