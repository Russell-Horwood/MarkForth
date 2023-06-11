using MarkForth.Storage;

namespace MarkForth.Scaffolds;

public interface IScaffoldService
{
    /// <summary>
    /// Gets the only usable <see cref="Scaffold"/>, 
    /// or returns <see langword="null"/> if there are none.
    /// </summary>
    /// <returns>
    /// The collection of Components that are referenced from <paramref name="input"/>.
    /// </returns>
    /// <exception cref="StorageException">
    /// An error occurred while loading a <see cref="Scaffold"/> from storage.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// More than one usable Scaffolds were found.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error occurred while interacting with MarkForth's storage system.
    /// </exception>
    internal Scaffold? Get();

    /// <summary>
    /// Inserts a scaffold into storage,
    /// which allows it to be used process MarkForth inputs.
    /// </summary>
    /// <param name="scaffold">
    /// The scaffold to insert.
    /// </param>
    /// <returns>
    /// <paramref name="scaffold"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="scaffold"/> is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// This store already contains a scaffold with the same name.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error occurred while interacting with MarkForth's storage system.
    /// </exception>
    Task<Scaffold> InsertAsync(Scaffold scaffold);
}
