using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace MarkForth.Storage.FileSystem;

internal sealed class FileSystemStoreBuilder<TEntity>
{
    private Collection<EntityFileMapping<TEntity>> fileMappings = new Collection<EntityFileMapping<TEntity>>();

    /// <summary>
    /// Gets the collection of mappings that have been configured using the <see cref="AddFile"/> method.
    /// </summary>
    internal ReadOnlyCollection<EntityFileMapping<TEntity>> FileMappings => fileMappings.AsReadOnly();

    /// <summary>
    /// Add a mapping that specifies where a stream property
    /// on instances of <see cref="TEntity"/>
    /// will be stored in the filing system.
    /// </summary>
    /// <param name="stream">
    /// Specifies the stream property to store.
    /// </param>
    /// <param name="fileName">
    /// A delegate that returns the name of the file 
    /// that will be used to store the stream contents
    /// for a given instance of <see cref="TEntity"/>.
    /// </param>
    /// <returns>
    /// This <see cref="FileSystemStoreBuilder{TEntity}"/>.
    /// </returns>
    internal FileSystemStoreBuilder<TEntity> AddFile(
        Expression<Func<TEntity, Stream?>> stream,
        Func<TEntity, string> fileName
    )
    {
        this.fileMappings.Add
        (
            new EntityFileMapping<TEntity>
            {
                FileName = fileName,
                Source = new Property<TEntity, Stream?>(stream)
            }
        );

        return this;
    }
}
