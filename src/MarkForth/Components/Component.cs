using System.ComponentModel.DataAnnotations;

namespace MarkForth.Components;

public sealed class Component : IDisposable
{

    #region Properties.

    [Key]
    public required string Name { get; init; }

    public Stream? Script { get; init; }

    public Stream? Style { get; init; }

    public required Stream Template { get; init; }

    #endregion Properties.

    #region IDisposable.

    private bool _disposed;

    private void dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Script?.Dispose();
                Style?.Dispose();
                Template?.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        dispose(true);
    }

    #endregion IDisposable.

}
