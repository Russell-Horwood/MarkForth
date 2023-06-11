using MarkForth.Components;
using MarkForth.Extensions.DependencyInjection;
using MarkForth.IO;
using MarkForth.Scaffolds;
using MarkForth.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace MarkForth.DependencyInjection;

internal sealed class MarkForthBuilder : IMarkForthBuilder
{

    #region Dependency Injection.

    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkForthBuilder"/> class
    /// with the specified services.
    /// </summary>
    /// <param name="services">The service collection to build upon.</param>
    public MarkForthBuilder(IServiceCollection services)
    {
        _services = services;
    }

    #endregion Dependency Injection.

    #region IMarkForthBuilder.

    public void AddFileSystemStores()
    {
        _services
            .AddFileSystemStore<Component>
            (
                "Components",
                builder => builder
                    .AddFile(c => c.Script, c => Path.ChangeExtension(c.Name, FileExtensions.JS))
                    .AddFile(c => c.Style, c => Path.ChangeExtension(c.Name, FileExtensions.CSS))
                    .AddFile(c => c.Template, c => Path.ChangeExtension(c.Name, FileExtensions.HTML))
            )
            .AddFileSystemStore<Scaffold>
            (
                "Scaffolds",
                builder => builder
                    .AddFile(s => s.Script, s => Path.ChangeExtension(s.Name ?? "default", FileExtensions.JS))
                    .AddFile(s => s.Style, s => Path.ChangeExtension(s.Name ?? "default", FileExtensions.CSS))
            );
    }

    public void AddInMemoryStores()
    {
        _services
            .AddSingleton<IStore<Component>, InMemoryStore<Component>>()
            .AddSingleton<IStore<Scaffold>, InMemoryStore<Scaffold>>();
    }

    #endregion IMarkForthBuilder.

}
