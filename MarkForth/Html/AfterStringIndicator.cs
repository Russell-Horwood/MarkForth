namespace MarkForth.Html;

/// <summary>
/// Receives individual characters from a sequence
/// and indicates when the current character is proceeded by a specific string of characters.
/// </summary>
internal sealed class AfterStringIndicator
{

    #region Dependency Injection.

    private readonly string _string;

    /// <summary>
    /// Creates a new instance of the <see cref="AfterStringIndicator"/> class.
    /// </summary>
    /// <param name="string">
    /// If the <see cref="Indicate"/> method has been called once
    /// for each of the characters of this string, in order,
    /// then the next call will return <see langword="true"/>.
    /// </param>
    internal AfterStringIndicator(string @string)
    {
        _string = @string;
    }

    #endregion Dependency Injection.

    #region Indicate.

    private bool _active;
    private int _stringIndex;

    /// <summary>
    /// Receives the next character from a sequence,
    /// and indicates if the this call was preceded by a call for each of the 
    /// sequence a characters determined by the string passed into the constructor.
    /// </summary>
    /// <param name="char">
    /// The next character from the sequence.
    /// </param>
    /// <returns>
    /// If this call has been preceded by a call for each of the sequence 
    /// of characters of the string passed into the constructor, in order,
    /// then the next call will return <see langword="true"/>.
    /// Otherwise returns <see langword="false"/>.
    /// </returns>
    internal bool Indicate(Char @char)
    {
        if (_active)
        {
            _active = false;
            _stringIndex = 0;
            return true;
        }

        if (@char != _string[_stringIndex])
        {
            _stringIndex = 0;
            return false;
        }

        _active = ++_stringIndex == _string.Length;
        return false;
    }

    #endregion Indicate.

}
