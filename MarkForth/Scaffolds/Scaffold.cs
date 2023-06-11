using System.ComponentModel.DataAnnotations;

namespace MarkForth.Scaffolds;

public sealed class Scaffold : IDisposable
{

    #region Properties.

    [Key]
    public string? Name { get; init; }

    public Stream? Script { get; init; }

    public Stream? Style { get; init; }

    #endregion Properties.

    #region IDisposable.

    private bool disposed;

    private void dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.Script?.Dispose();
                this.Style?.Dispose();
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.dispose(true);
    }

    #endregion IDisposable.

}
