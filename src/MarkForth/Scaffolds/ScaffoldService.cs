using MarkForth.Storage;

namespace MarkForth.Scaffolds;

internal sealed class ScaffoldService : IScaffoldService
{

    #region Dependency Injection.

    private readonly IStore<Scaffold> scaffolds;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaffoldService"/> class.
    /// </summary>
    /// <param name="scaffolds">
    /// A store to read/write scaffolds from/to.
    /// </param>
    /// <paramref name="scaffolds"/> is <see langword="null"/>.
    /// </exception>
    public ScaffoldService(
        IStore<Scaffold> scaffolds
    )
    {
        this.scaffolds = scaffolds;
    }

    #endregion DependencyInjection.

    #region IScaffoldService.

    public Scaffold? Get()
    {
        return this.scaffolds.SingleOrDefault();
    }

    public Task<Scaffold> InsertAsync(Scaffold scaffold)
    {
        ArgumentNullException.ThrowIfNull(scaffold);

        if (this.scaffolds.Any(s => s.Name == scaffold.Name))
            throw new InvalidOperationException($"There already exists a scaffold with name '{scaffold.Name}'.");

        return this.scaffolds.InsertAsync(scaffold);
    }

    #endregion IScaffoldService.

}
