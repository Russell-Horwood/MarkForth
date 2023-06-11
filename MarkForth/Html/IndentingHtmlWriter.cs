namespace MarkForth.Html;

internal sealed class IndentingHtmlWriter : HtmlWriter
{

    #region Dependency Injection.

    private readonly HtmlContext _context;

    public IndentingHtmlWriter(
        HtmlContext context,
        TextWriter innerWriter
    ) : base(context, innerWriter)
    {
        _context = context;
    }

    #endregion Dependency Injection.

    private readonly Queue<char> _buffer = new(2);

    #region WriteHtml.

    private int _indentationLevel;
    private int _previousLineDepth;
    private int _currentLineDepth;

    protected override void WriteHtml(char value)
    {
        if (_context.States.Current.PreserveWhitespace || _context.States.Previous.PreserveWhitespace)
        {
            base.WriteHtml(value);
            return;
        }

        switch (_context.States.Current.Line)
        {
            case HtmlContext.LineState.StartingLine:
                startLine(value);
                return;

            default:
                switch (_context.States.Previous.Line)
                {
                    case HtmlContext.LineState.StartingLine:
                        startedLine(value);
                        break;

                    case HtmlContext.LineState.OpeningTag:
                        if (_context.States.Current.Line != HtmlContext.LineState.OpeningTag)
                            _currentLineDepth++;
                        break;

                    case HtmlContext.LineState.ClosingTag:
                        if (_context.States.Current.Line != HtmlContext.LineState.ClosingTag)
                            _currentLineDepth--;
                        break;
                }
                flush();
                base.WriteHtml(value);
                return;
        }
    }

    private void startLine(char value)
    {
        if (!Char.IsWhiteSpace(value))
            _buffer.Enqueue(value);
    }

    private void startedLine(char value)
    {
        // Evaluate how the depth changed on the previous line.
        if (_currentLineDepth > _previousLineDepth)
        {
            _indentationLevel++;
            _previousLineDepth = _currentLineDepth;
        }
        else if (_currentLineDepth < _previousLineDepth)
        {
            _indentationLevel--;
            _previousLineDepth = _currentLineDepth;
        }

        // Write the buffered content up where the indentation should be written.
        flush(1);

        // Write the indentation.
        writeIndents
        (
            _context.States.Current.Line == HtmlContext.LineState.StartingTag
                && value == '/'
                    ? -1
                    : 0
        );
    }

    private void writeIndents(int offset = 0)
    {
        base.Write
        (
            String.Join
            (
                "",
                Enumerable.Repeat("  ", _indentationLevel + offset)
            )
        );
    }

    #endregion WriteHtml.

    public override void Close()
    {
        this.flush();
        base.Close();
    }

    private void flush(uint untilCount = 0)
    {
        while (_buffer.Count > untilCount)
            base.WriteHtml(_buffer.Dequeue());
    }
}
