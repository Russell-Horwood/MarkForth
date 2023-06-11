namespace MarkForth.IO;

public interface IFile
{
    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">
    /// The file to check.
    /// </param>
    /// <returns>
    /// <para>
    /// <see langword="true"/> if the caller has the required permissions
    /// and <paramref name="path"/> contains the name of an existing file;
    /// otherwise, <see langword="false"/>.
    /// </para>
    /// <para>
    /// This method also returns <see langword="false"/> if <paramref name="path"/> is
    /// <see langword="null"/>, an invalid path, or a zero-length <see langword="string"/>.
    /// </para>
    /// <para>
    /// If the caller does not have sufficient permissions to read the specified file,
    /// no exception is thrown and the method returns <see langword="false"/> regardless of the existence of path.
    /// </para>
    /// </returns>
    bool Exists(string? path);

    /// <summary>
    /// Opens an existing file for reading.
    /// </summary>
    /// <param name="path">
    /// The file to be opened for reading.
    /// </param>
    /// <returns>
    /// A read-only <see cref="FileStream"/> on the specified path.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="PathTooLongException">
    /// The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// The specified path is invalid, (for example, it is on an unmapped drive).
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// <paramref name="path"/> specified a directory.
    /// -or- The caller does not have the required permission.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// The file specified in <paramref name="path"/> was not found.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="path"/> is in an invalid format.
    /// </exception>
    /// <exception cref="IOException">
    /// An I/O error occurred while opening the file.
    /// </exception>
    Stream OpenRead(string path);

    /// <summary>
    ///  Opens an existing file or creates a new file for writing.
    /// </summary>
    /// <param name="path">The file to be opened for writing.</param>
    /// <returns>
    ///  An unshared <see cref="FileStream"/> object on the specified path with <see cref="FileAccess.Write"/>
    /// access.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// <br/>-or- <paramref name="path"/> specified a read-only file or directory. 
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="PathTooLongException">
    /// The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// The specified path is invalid, (for example, it is on an unmapped drive).
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="path"/> is in an invalid format.
    /// </exception>
    Stream OpenWrite(string path);
}
