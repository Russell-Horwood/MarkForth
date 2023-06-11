namespace MarkForth.Html;

internal sealed class ImgHtmlContext : HtmlContext
{
    internal override void Update(char value)
    {
        base.Update(value);
        updateIsInImgSrc(value);
    }

    #region updateIsInImgSrc.

    private enum imgState
    {
        beforeOpenTag,
        insideOpenTag,
        insideSrcAttribute
    }
    private imgState _imgState;
    private readonly AfterStringIndicator _startOpenImg = new($"<{ElementNames.Img}");
    private const char _endOpenImg = '>';
    private readonly AfterStringIndicator _openSrcAttribute = new(" src=\"");
    private readonly char _closeSrcAttribute = '"';

    internal bool IsInImgSrc { get; private set; }

    private void updateIsInImgSrc(char value)
    {
        switch (_imgState)
        {
            case imgState.beforeOpenTag:
                beforeOpenTag(value);
                break;

            case imgState.insideOpenTag:
                insideOpenTag(value);
                break;

            case imgState.insideSrcAttribute:
                insideSrcAttribute(value);
                break;
        }

        IsInImgSrc = _imgState == imgState.insideSrcAttribute;
    }

    private void beforeOpenTag(char @char)
    {
        if (_startOpenImg.Indicate(@char))
        {
            _imgState = imgState.insideOpenTag;
            insideOpenTag(@char);
        }
    }

    private void insideOpenTag(char @char)
    {
        if (_openSrcAttribute.Indicate(@char))
        {
            _imgState = imgState.insideSrcAttribute;
            insideSrcAttribute(@char);
        }
        else if (@char == _endOpenImg)
        {
            _imgState = imgState.beforeOpenTag;
            beforeOpenTag(@char);
        }
    }

    private void insideSrcAttribute(char @char)
    {
        if (@char == _closeSrcAttribute)
        {
            _imgState = imgState.beforeOpenTag;
            beforeOpenTag(@char);
        }
    }

    #endregion updateIsInImgSrc.

}
