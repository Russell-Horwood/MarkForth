namespace MarkForth.DependencyInjection;

public interface IMarkForthBuilder
{
    /// <summary>
    /// Adds file system storage of MarkForth entities (e.g. Scaffolds and Components).
    /// </summary>
    void AddFileSystemStores();

    /// <summary>
    /// Adds in-memory storage of MarkForth entities (e.g. Scaffolds and Components).
    /// </summary>
    void AddInMemoryStores();
}
