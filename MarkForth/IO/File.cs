namespace MarkForth.IO;

internal sealed class File : IFile
{
    public bool Exists(string? path)
    {
        return System.IO.File.Exists(path);
    }

    public Stream OpenRead(string path)
    {
        return System.IO.File.OpenRead(path);
    }

    public Stream OpenWrite(string path)
    {
        return System.IO.File.OpenWrite(path);
    }
}
