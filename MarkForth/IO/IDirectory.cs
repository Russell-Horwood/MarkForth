using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace MarkForth.IO;

internal interface IDirectory
{
    /// <summary>
    /// Creates all directories and subdirectories in the specified path unless they
    /// already exist.
    /// </summary>
    /// <param name="path">The directory to create.</param>
    /// <returns>
    /// An object that represents the directory at the specified path. This object is
    /// returned regardless of whether a directory at the specified path already exists.
    /// </returns>
    /// <exception cref="IOException">
    /// The directory specified by <paramref name="path"/> is a file.
    /// -or- The network name is not known.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="path"/> is prefixed with, or contains, only a colon character (:).
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="PathTooLongException">
    /// The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// The specified path is invalid (for example, it is on an unmapped drive).
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="path"/> contains a colon character (:) that is not part of a drive label ("C:\").
    /// </exception>
    DirectoryInfo CreateDirectory(string path);

    /// <summary>
    /// Gets the current working directory of the application.
    /// </summary>
    /// <returns>
    /// A string that contains the absolute path of the current working directory,
    /// and does not end with a backslash (\).
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The operating system is Windows CE, which does not have current directory functionality.
    /// This method is available in the .NET Compact Framework,
    /// but is not currently supported.
    /// </exception>
    string CurrentDirectory { get; }

    /// <summary>
    /// Returns an enumerable collection of directory full names in a specified path.
    /// </summary>
    /// <param name="path">
    /// The relative or absolute path to the directory to search.
    /// This string is not case-sensitive.
    /// </param>
    /// <returns>
    /// An enumerable collection of the full names (including paths) for the directories
    /// in the directory specified by <paramref name="path"/>.
    /// </returns>
    /// <exception type="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// <paramref name="path"/> is invalid, such as referring to an unmapped drive.
    /// </exception>
    /// <exception cref="IOException">
    /// <paramref name="path"/> is a file name.
    /// </exception>
    /// <exception cref="PathTooLongException">
    /// The specified path, file name, or combined exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="SecurityException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    IEnumerable<string> EnumerateDirectories(string path);

    /// <summary>
    /// Returns an enumerable collection of full file names in a specified path.
    /// </summary>
    /// <param name="path">
    /// The relative or absolute path to the directory to search.
    /// This string is not case-sensitive.
    /// </param>
    /// <returns>
    /// An enumerable collection of the full names (including paths)
    /// for the files in the directory specified by path.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// <paramref name="path"/> is invalid, such as referring to an unmapped drive.
    /// </exception>
    /// <exception cref="IOException">
    /// <paramref name="path"/> is a file name.
    /// </exception>
    /// <exception cref="PathTooLongException">
    /// The specified path, file name, or combined exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="SecurityException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    IEnumerable<string> EnumerateFiles(string path);

    /// <summary>
    /// Determines whether the given path refers to an existing directory on disk.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="path"/> refers to an existing directory;
    /// <see langword="false" /> if the directory does not exist or an error occurs
    /// when trying to determine if the specified directory exists.
    /// </returns>
    bool Exists([NotNullWhen(true)] string? path);
}
