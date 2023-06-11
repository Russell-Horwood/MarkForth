namespace MarkForth.Storage.FileSystem;

internal sealed class EntityFileMapping<TEntity>
{
    /// <summary>
    /// A property on <see cref="TEntity"/> that specifies
    /// the name of the file that holds the contents of <see cref="Source"/>.
    /// </summary>
    internal required Func<TEntity, string> FileName { get; init; }

    /// <summary>
    /// A property on <see cref="TEntity"/> that streams
    /// the contents of the file specified by <see cref="FileName"/>.
    /// </summary>
    internal required Property<TEntity, Stream?> Source { get; init; }
}
