using MarkForth.Storage;
using System.Security;
using System.Xml;

namespace MarkForth.Components;

public interface IComponentService
{
    /// <summary>
    /// Reads the input stream to find and load the Components it references.
    /// </summary>
    /// <param name="input">
    /// A MarkForth input stream.
    /// </param>
    /// <remarks>
    /// The position of <paramref name="input"/> will be advanced to the end.
    ///</remarks>
    /// <returns>The collection of Components that are referenced from <paramref name="input"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="input"/> is <see langword="null" />.
    /// </exception>
    /// <exception cref="XmlException">
    /// The content of <paramref name="input"/> is not valid XML.
    /// </exception>
    /// <exception cref="SecurityException">
    /// The caller does not have sufficient permissions to access the source location of <paramref name="input"/>.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error occurred while interacting with MarkForth's storage system.
    /// </exception>
    internal IReadOnlyCollection<Component> Get(Stream input);

    /// <summary>
    /// Inserts a component into storage,
    /// which allows it to be referenced by MarkForth inputs.
    /// </summary>
    /// <param name="component">
    /// The component to insert.</param>
    /// <returns>
    /// <paramref name="component"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="component"/> is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// This store already contains a component with the same name.
    /// </exception>
    /// <exception cref="StorageException">
    /// An error occurred while interacting with MarkForth's storage system.
    /// </exception>
    Task<Component> InsertAsync(Component component);
}
