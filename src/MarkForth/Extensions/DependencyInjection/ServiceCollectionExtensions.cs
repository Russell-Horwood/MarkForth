using MarkForth.Components;
using MarkForth.DependencyInjection;
using MarkForth.Html;
using MarkForth.IO;
using MarkForth.Processors;
using MarkForth.Scaffolds;
using MarkForth.Storage;
using MarkForth.Storage.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Directory = MarkForth.IO.Directory;
using File = MarkForth.IO.File;

namespace MarkForth.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{

    #region AddMarkForth.

    /// <summary>
    /// Adds processing of MarkForth files to a service collection,
    /// with file system storage of entities (e.g. Scaffolds and Components).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns><paramref name="services"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="services"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection AddMarkForth(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddMarkForth(builder => builder.AddFileSystemStores());
    }

    /// <summary>
    /// Adds processing of MarkForth files to a service collection,
    /// without any storage of entities (e.g. Scaffolds and Components).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configures the services. E.g. Add a storage layer.</param>
    /// <returns><paramref name="services"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="services"/> or <paramref name="configure"/> are <see langword="null"/>.
    /// </exception>
    public static IServiceCollection AddMarkForth(this IServiceCollection services, Action<IMarkForthBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services
            .addFileSystem()
            .AddLogging()
            .AddTransient<ImgHtmlContext>()
            .AddTransient<HtmlContext>()
            .AddSingleton<IComponentService, ComponentService>()
            .AddSingleton<IProcessor, Processor>()
            .AddSingleton<IScaffoldService, ScaffoldService>();

        configure(new MarkForthBuilder(services));

        return services;
    }

    /// <summary>
    /// Adds services for working with the local file system via MarkForth abstractions to a service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns><paramref name="services"/>.</returns>
    private static IServiceCollection addFileSystem(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDirectory, Directory>()
            .AddSingleton<IFile, File>();
    }

    #endregion AddMarkForth.

    /// <summary>
    /// Adds registrations to the specified service collection
    /// for using the file system for storage of the specified type of entity.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity to configure file system storage for.
    /// </typeparam>
    /// <param name="services">
    /// The service collection to add the registrations to.
    /// </param>
    /// <param name="baseFolderName">
    /// The name or path of a folder, relative to the working directory,
    /// in which to read/write entity data.
    /// </param>
    /// <param name="configure">
    /// An action that configures mappings between properties of <paramref name="TEntity"/>
    /// and file name/paths inside <paramref name="baseFolderName"/>
    /// that will store the property values.
    /// </param>
    /// <returns>
    /// <paramref name="services"/>
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="configure"/> did not configure any storage.
    /// </exception>
    internal static IServiceCollection AddFileSystemStore<TEntity>(
        this IServiceCollection services,
        string baseFolderName,
        Action<FileSystemStoreBuilder<TEntity>> configure
    ) where TEntity : class
    {
        FileSystemStoreBuilder<TEntity> builder = new();
        configure(builder);
        if (!builder.FileMappings.Any())
            throw new ArgumentException("No files were mapped to the file system store.");

        return services.AddSingleton<IStore<TEntity>>
        (
            serviceProvider =>
            {
                return ActivatorUtilities.CreateInstance<FileSystemStore<TEntity>>
                (
                    serviceProvider,
                    baseFolderName,
                    builder.FileMappings
                );
            }
        );
    }
}
