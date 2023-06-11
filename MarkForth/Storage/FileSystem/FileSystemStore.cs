using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Text;
using MarkForth.IO;

namespace MarkForth.Storage.FileSystem;

internal sealed class FileSystemStore<TEntity>
    : Store<TEntity>
    where TEntity : class
{
    static FileSystemStore()
    {
        if (IStore<TEntity>.EntityType.IsAbstract)
        {
            throw new ArgumentException
            (
                $"Entity type '{typeof(TEntity).FullName}' cannot be stored because on the File System because it is an abstract type.",
                nameof(TEntity)
            );
        }

        if (IStore<TEntity>.EntityType.GetConstructor(Array.Empty<Type>()) == null)
        {
            throw new ArgumentException
            (
                $"Entity type '{typeof(TEntity).FullName}' cannot be stored because on the File System because it does not have an accessible parameterless constructor.",
                nameof(TEntity)
            );
        }
    }

    #region Dependency Injection.

    private readonly string baseFolderName;
    private readonly IDirectory directory;
    private readonly IFile file;
    private readonly IEnumerable<EntityFileMapping<TEntity>> fileMappings;

    public FileSystemStore(
        string baseFolderName,
        IDirectory directory,
        IFile file,
        IEnumerable<EntityFileMapping<TEntity>> fileMappings
    )
    {
        this.baseFolderName = baseFolderName;
        this.directory = directory;
        this.file = file;
        this.fileMappings = fileMappings;
    }

    #endregion Dependency Injection.

    #region BaseFolderPath.

    private string? baseFolderPathField;
    private string baseFolderPath
    {
        get
        {
            if (this.baseFolderPathField == null)
            {
                this.baseFolderPathField = Path.Combine
                (
                    Path.GetDirectoryName(typeof(FileSystemStore<TEntity>).Assembly.Location)!,
                    this.baseFolderName
                );
            }
            return this.baseFolderPathField;
        }
    }

    #endregion BaseFolderPath.

    #region Store.

    #region InsertAsyncInternal.

    protected override async Task<TEntity> InsertInternalAsync(TEntity entity)
    {
        string entityFolderPath = Path.Combine(this.baseFolderPath, IStore<TEntity>.Key.Get(entity));

        this.createFolder(entityFolderPath);

        await Task.WhenAll
        (
            this.fileMappings
                .Select(mapping => (fileName: mapping.FileName(entity), source: mapping.Source.Get(entity)))
                .Where(tuple => tuple.source != null)
                .Select(tuple =>
                {
                    using Stream target = this.openWrite(Path.Combine(entityFolderPath, tuple.fileName));
                    return tuple.source!.CopyToAsync(target);
                })
        )
        .ConfigureAwait(false);

        return entity;
    }

    private void createFolder(string path)
    {
        Func<Exception, StorageException> @catch = exception =>
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"An error occurred inserting an entity of type '{this.ElementType}.'");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"While creating the folder '{path}'.");
            return new StorageException(messageBuilder.ToString(), exception);
        };

        try
        {
            this.directory.CreateDirectory(path);
        }
        catch (UnauthorizedAccessException exception) { throw @catch(exception); }
        catch (ArgumentException exception) { throw @catch(exception); }
        catch (PathTooLongException exception) { throw @catch(exception); }
        catch (NotSupportedException exception) { throw @catch(exception); }
    }

    private Stream openWrite(string path)
    {
        Func<Exception, StorageException> @catch = exception =>
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"An error occurred inserting an entity of type '{this.ElementType}.'");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"While writing the file '{path}'.");
            return new StorageException(messageBuilder.ToString(), exception);
        };

        try
        {
            return this.file.OpenWrite(path);
        }
        catch (PathTooLongException exception) { throw @catch(exception); }
        catch (NotSupportedException exception) { throw @catch(exception); }
    }

    #endregion InsertAsync.

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

    private IQueryable<TEntity> query => this.enumerate().AsQueryable();

    private IEnumerable<TEntity> enumerate()
    {
        return this
            .enumerateFolders()
            .Select(entityFolderPath =>
            {
                string key = new DirectoryInfo(entityFolderPath).Name;
                TEntity entity = Activator.CreateInstance<TEntity>();
                this.setKey(entity, key);

                foreach (EntityFileMapping<TEntity> fileMapping in this.fileMappings)
                {
                    string fileName = Path.Combine(entityFolderPath, fileMapping.FileName(entity));
                    if (file.Exists(fileName))
                    {
                        this.setProperty
                        (
                            entity,
                            fileMapping,
                            key,
                            this.openRead
                            (
                                key,
                                fileMapping,
                                Path.Combine(entityFolderPath, fileName)
                            )
                        );
                    }
                    else if (!fileMapping.Source.IsNullable)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"An error occurred loading entity of type '{this.ElementType}.'");
                        messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"With key '{key}'.");
                        messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"While initializing property '{fileMapping.Source.Name}'.");
                        messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"Required file not found at path: '{entityFolderPath}'.");
                        throw new StorageException(messageBuilder.ToString());
                    }
                }

                return entity;
            });
    }

    private IEnumerable<string> enumerateFolders()
    {
        Func<Exception, Exception> @catch = exception =>
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"An error occurred loading entities of type '{this.ElementType}.'");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"While reading the folders inside '{this.baseFolderPath}'.");
            return new StorageException(messageBuilder.ToString(), exception);
        };

        try
        {
            return this.directory.EnumerateDirectories(this.baseFolderPath);
        }
        catch (DirectoryNotFoundException exception) { throw @catch(exception); }
        catch (IOException exception) { throw @catch(exception); }
        catch (SecurityException exception) { throw @catch(exception); }
        catch (UnauthorizedAccessException exception) { throw @catch(exception); }
    }

    private Stream openRead(string key, EntityFileMapping<TEntity> fileMapping, string path)
    {
        Func<Exception, Exception> @catch = exception =>
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"An error occurred loading entity of type '{this.ElementType}.'");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"With key '{key}'.");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"While initializing property '{fileMapping.Source.Name}'.");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"While opening file path for reading '{path}'.");
            return new StorageException(messageBuilder.ToString(), exception);
        };

        try
        {
            return this.file.OpenRead(path);
        }
        catch (PathTooLongException exception) { throw @catch(exception); }
        catch (DirectoryNotFoundException exception) { throw @catch(exception); }
        catch (UnauthorizedAccessException exception) { throw @catch(exception); }
        catch (FileNotFoundException exception) { throw @catch(exception); }
        catch (NotSupportedException exception) { throw @catch(exception); }
        catch (IOException exception) { throw @catch(exception); }
    }

    private void setKey(TEntity entity, string key)
    {
        try
        {
            IStore<TEntity>.Key.Set(entity, key);
        }
        catch (TargetInvocationException exception)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"An error occurred loading entity of type '{this.ElementType}.'");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"With key '{key}'.");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"While setting the key property '{IStore<TEntity>.Key.Name}'.");
            throw new StorageException(messageBuilder.ToString(), exception);
        }
    }

    private void setProperty(TEntity entity, EntityFileMapping<TEntity> fileMapping, string key, Stream stream)
    {
        try
        {
            fileMapping.Source.Set(entity, stream);
        }
        catch (TargetInvocationException exception)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"An error occurred loading entity of type '{this.ElementType}.'");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"With key '{key}'.");
            messageBuilder.AppendLine(CultureInfo.InvariantCulture, $"While setting the property '{fileMapping.Source.Name}'");
            throw new StorageException(messageBuilder.ToString(), exception);
        }
    }

    #endregion IQueryable<TEntity>

    #endregion Store.

}
