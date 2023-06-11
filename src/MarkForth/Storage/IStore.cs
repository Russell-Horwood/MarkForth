using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MarkForth.Storage;

internal interface IStore<TEntity> : IQueryable<TEntity>
    where TEntity : class
{
    protected static Type EntityType { get; } = typeof(TEntity);

    #region Key.

    protected static Property<TEntity, string> Key { get; }

    /// <summary>
    /// Initialises the <see cref="IStore"/> type.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// <see cref="TEntity"/> does not have exactly one property decorated with <see cref="KeyAttribute"/>
    /// that has both get and set methods defined and accessible.
    /// </exception>
    static IStore()
    {
        PropertyInfo[] keyInfos = EntityType
            .GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
            .Take(2)
            .ToArray();

        if (keyInfos.Length == 0)
        {
            throw new ArgumentException
            (
                $"Entity type '{typeof(TEntity).FullName}' cannot be stored because it does not have any keys defined.",
                nameof(TEntity)
            );
        }

        if (keyInfos.Length == 2)
        {
            throw new ArgumentException
            (
                $"Entity type '{typeof(TEntity).FullName}' cannot be stored because it have more than one key defined.",
                nameof(TEntity)
            );
        }

        if (keyInfos[0].GetGetMethod() == null)
        {
            throw new ArgumentException
            (
                $"Entity type '{typeof(TEntity).FullName}' cannot be stored because it's key property '{keyInfos[0].Name}' does not have an accessible get method.",
                nameof(TEntity)
            );
        }

        if (keyInfos[0].GetSetMethod() == null)
        {
            throw new ArgumentException
            (
                $"Entity type '{typeof(TEntity).FullName}' cannot be stored because it's key property '{keyInfos[0].Name}' does not have an accessible set method.",
                nameof(TEntity)
            );
        }

        Key = new Property<TEntity, string>(keyInfos[0]);
    }

    #endregion Key.

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
    Task<TEntity> InsertAsync(TEntity entity);

}
