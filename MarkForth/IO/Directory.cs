namespace MarkForth.IO;

internal sealed class Directory : IDirectory
{
    public DirectoryInfo CreateDirectory(string path)
    {
        return System.IO.Directory.CreateDirectory(path);
    }

    public string CurrentDirectory
    {
        get => System.IO.Directory.GetCurrentDirectory();
    }

    public IEnumerable<string> EnumerateDirectories(string path)
    {
        return System.IO.Directory.EnumerateDirectories(path);
    }

    public IEnumerable<string> EnumerateFiles(string path)
    {
        return System.IO.Directory.EnumerateFiles(path);
    }

    public bool Exists(string? path)
    {
        return System.IO.Directory.Exists(path);
    }
}
