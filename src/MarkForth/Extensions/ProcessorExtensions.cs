using MarkForth.Extensions.IO;
using MarkForth.Processors;
using MarkForth.Storage;
using System.Xml;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace MarkForth.Extensions;

public static class ProcessorExtensions
{
    /// <summary>
    /// Processes a MarkForth input file,
    /// and writes the resultant HTML to the specified output file,
    /// using the specified MarkForth Processor.
    /// </summary>
    /// <param name="processor">
    /// An <see cref="IProcessor"/> implementation to perform the processing.
    /// </param>
    /// <param name="inputPath">
    /// The path to the MarkForth input file.
    /// </param>
    /// <param name="outputPath">
    /// The path to the output file to write the resultant HTML to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="processor"/> is <see langword="null" />.
    /// -or- <paramref name="inputPath"/> is <see langword="null" />.
    /// -or <paramref name="outputPath)"/> is <see langword="null" />.
    /// </exception>
    /// <exception cref="PathTooLongException">
    /// <paramref name="inputPath" /> exceeds the system-defined maximum length.
    /// -or <paramref name="outputPath"/> exceeds the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// <paramref name="inputPath"/> is invalid, (for example, it is on an unmapped drive).
    /// -or- <paramref name="outputPath"/> is invalid (for example, it is on an unmapped drive).
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// <paramref name="inputPath"/> specified a directory.
    /// -or- The caller does not have the required permission to read <paramref name="inputPath"/>.
    /// -or- <paramref name="outputPath"/> specified a file that is read-only. 
    /// -or- <paramref name="outputPath"/> specified a file that is hidden.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// The file specified in <paramref name="inputPath"/> was not found.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="inputPath"/> is in an invalid format.
    /// -or- <paramref name="outputPath"/> is in an invalid format.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while opening the file specified by <paramref name="inputPath"/>.
    /// -or An I/O error occurred while creating the file specified by <paramref name="outputPath"/>.
    /// -or An I/O error occurs while writing to the file specified by <paramref name="outputPath"/>.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error occurred while interacting with MarkForth's storage system.
    /// </exception>
    /// <exception cref="XmlException">
    /// The content of the file specified by <paramref name="inputPath"/> is not valid XML.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// More than one usable Scaffolds were found.
    /// </exception>
    public static void ProcessFileToFile(this IProcessor processor, string inputPath, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(processor);
        ArgumentNullException.ThrowIfNull(outputPath);

        using FileStream input = File.OpenRead(inputPath);
        using Stream output = File.Create(outputPath);
        processor.Process(input, output);
    }

    /// <summary>
    /// Processes a MarkForth input string,
    /// and returns a MemoryStream to which the resultant HTML has been written.
    /// </summary>
    /// <param name="processor">
    /// An <see cref="IProcessor"/> implementation to perform the processing.
    /// </param>
    /// <param name="input">
    /// The MarkForth input string.
    /// </param>
    /// <returns>
    /// A MemoryStream to which the resultant HTML has been written.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="processor"/> is <see langword="null" />.
    /// -or- <paramref name="input"/> is <see langword="null" />.
    /// </exception>
    /// <exception cref="XmlException">
    /// <paramref name="input"/> is not valid XML.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// More than one usable Scaffolds were found.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error occurred while interacting with MarkForth's storage system.
    /// </exception>
    public static MemoryStream ProcessStringToNewStream(this IProcessor processor, string input)
    {
        ArgumentNullException.ThrowIfNull(processor);
        ArgumentNullException.ThrowIfNull(input);

        using MemoryStream inputStream = input.ToMemoryStream();
        MemoryStream outputStream = new MemoryStream();
        processor.Process(inputStream, outputStream);

        outputStream.Position = 0;
        return outputStream;
    }

    /// <summary>
    /// Processes a MarkForth input string,
    /// and returns the resultant HTML as a string.
    /// </summary>
    /// <param name="processor">
    /// An <see cref="IProcessor"/> implementation to perform the processing.
    /// </param>
    /// <param name="input">
    /// The MarkForth input string.
    /// </param>
    /// <returns>
    /// The HTML output string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="processor"/> is <see langword="null" />.
    /// -or- <paramref name="input"/> is <see langword="null" />.
    /// </exception>
    /// <exception cref="XmlException">
    /// <paramref name="input"/> is not valid XML.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// More than one usable Scaffolds were found.
    /// </exception>
    /// <exception cref="OutOfMemoryException">
    /// There is insufficient memory to allocate a buffer for the returned string.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error occurred while interacting with MarkForth's storage system.
    /// </exception>
    public static string ProcessStringToString(this IProcessor processor, string input)
    {
        ArgumentNullException.ThrowIfNull(processor);
        ArgumentNullException.ThrowIfNull(input);

        return processor.ProcessStringToNewStream(input).ReadAsString();
    }
}
