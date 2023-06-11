using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace MarkForth.Storage;

internal sealed class InMemoryStore<TEntity>
    : Store<TEntity>
    where TEntity : class
{

    #region Store.

    protected override Task<TEntity> InsertInternalAsync(TEntity entity)
    {
        this.entities.Add(entity);
        return Task.FromResult(entity);
    }

    #region IQueryable<TEntity>.

    #region IQueryable.

    public override Expression Expression => this.query.Expression;

    public override IQueryProvider Provider => this.query.Provider;

    #endregion IQueryable.

    #region IEnumerable<TEntity>.

    public override IEnumerator<TEntity> GetEnumerator()
    {
        return this.query.GetEnumerator();
    }

    #endregion IEnumerable<TEntity>.

    #endregion IQueryable<TEntity>.

    private IQueryable<TEntity> query => this.entities.AsQueryable();

    private readonly List<TEntity> entities = new List<TEntity>();

    #endregion Store.

}
