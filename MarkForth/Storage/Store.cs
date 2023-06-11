using System.Collections;
using System.Linq.Expressions;

namespace MarkForth.Storage;

internal abstract class Store<TEntity>
    : IStore<TEntity>
    where TEntity : class
{

    #region IStore<TEntity>.

    #region InsertAsync<TEntity>.

    private static string entityName = IStore<TEntity>.EntityType.Name;

    private static Property<TEntity, string> key = IStore<TEntity>.Key;

    private static string keyName = IStore<TEntity>.Key.Name;

    /// <summary>
    /// Inserts the specified entity into this store asynchronously.
    /// </summary>
    /// <param name="entity">
    /// The entity to insert.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TEntity}"/> that returns <paramref name="entity"/> when it has been inserted.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entity"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// This store already contains an entity with the same key.
    /// </exception>
    public Task<TEntity> InsertAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        string keyValue = key.Get(entity);
        if (this.Any(e => key.Get(e) == keyValue))
        {
            throw new InvalidOperationException
            (
                $"Cannot insert {entityName} with {keyName} '{keyValue}' because a {entityName} with that {keyName} already exists."
            );
        }

        return this.InsertInternalAsync(entity);
    }

    /// <summary>
    /// Inserts the specified entity into this store asynchronously.
    /// </summary>
    /// <param name="entity">
    /// The entity to insert. Will not be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TEntity}"/> that returns <paramref name="entity"/> when it has been inserted.
    /// </returns>
    protected abstract Task<TEntity> InsertInternalAsync(TEntity entity);

    #endregion InsertAsync<TEntity>.

    #region IQueryable<TEntity>.

    #region IEnumerable<TEntity>.

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IEnumerable<TEntity>.

    #region IEnumerable.

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    public abstract IEnumerator<TEntity> GetEnumerator();

    #endregion IEnumerable.

    #region IQueryable.

    /// <summary>
    ///  Gets the type of the element(s) that are returned when the expression tree associated
    ///  with this instance of System.Linq.IQueryable is executed.
    /// </summary>
    public Type ElementType { get; } = IStore<TEntity>.EntityType;

    /// <summary>
    /// Gets the expression tree that is associated with the instance of <see cref="IQueryable"/>.
    /// </summary>
    public abstract Expression Expression { get; }

    /// <summary>
    ///  Gets the query provider that is associated with this data source.
    /// </summary>
    public abstract IQueryProvider Provider { get; }

    #endregion IQueryable.

    #endregion IQueryable<TEntity>.

    #endregion IStore<TEntity>.

}
