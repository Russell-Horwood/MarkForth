namespace MarkForth.Html;

internal sealed class ScriptElementWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly HeadElementWriter _headElementWriter;

    internal ScriptElementWriter(
        HtmlContext context,
        HeadElementWriter headElementWriter
    ) : base(context, headElementWriter)
    {
        _headElementWriter = headElementWriter;
    }

    #endregion Dependency Injection.

    internal HeadElementWriter WriteScriptEnd()
    {
        WriteElementEndLine(ElementNames.Script);
        return _headElementWriter;
    }
}
