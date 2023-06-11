namespace MarkForth.Storage;

public class StorageException : MarkForthException
{

    #region Construction.

    internal StorageException()
        : base(H_RESULT) { }

    internal StorageException(string message)
        : base(H_RESULT, message) { }

    internal StorageException(string message, Exception innerException)
        : base(H_RESULT, message, innerException) { }

    // https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/0642cb2f-2075-4469-918c-4441e69c548a
    private const uint H_RESULT = 0b10100000000000110000000000000000;

    #endregion Construction.

}
