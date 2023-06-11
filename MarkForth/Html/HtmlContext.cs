namespace MarkForth.Html
{
    internal class HtmlContext
    {
        internal virtual void Update(char value)
        {
            updateLineState(value);
            updatePreserveWhitespace(value);
        }

        #region (Html)States.

        private HtmlStates _states = new();
        internal IHtmlStates States => _states;

        internal interface IHtmlStates
        {
            IHtmlState Current { get; }

            IHtmlState Previous { get; }
        }

        #region (Html)State.

        private sealed class HtmlStates : IHtmlStates
        {
            internal HtmlState _current = new();
            public IHtmlState Current => _current;

            internal HtmlState _previous = new();
            public IHtmlState Previous => _previous;
        }

        internal interface IHtmlState
        {
            LineState Line { get; }

            bool PreserveWhitespace { get; }
        }

        private sealed class HtmlState : IHtmlState
        {
            internal LineState _line;
            public LineState Line => _line;

            internal bool _preserveWhitespace;
            public bool PreserveWhitespace => _preserveWhitespace;
        }

        #region PreserveWhitespace.

        private readonly AfterStringIndicator _pre = new($"{ElementNames.Pre}>");

        private void updatePreserveWhitespace(char @char)
        {
            _states._previous._preserveWhitespace = _states.Current.PreserveWhitespace;

            if (_pre.Indicate(@char))
            {
                _states._current._preserveWhitespace = _states._previous._line switch
                {
                    LineState.OpeningTag => true,
                    LineState.ClosingTag => false,
                    _ => _states._current._preserveWhitespace
                };
            }
        }

        #endregion PreserveWhitespace.

        #region LineState.

        internal enum LineState
        {
            StartingLine,
            BeforeStartTag,
            StartingTag,
            OpeningTag,
            ClosingTag
        }

        #region updateLineState.

        private void updateLineState(char @char)
        {
            _states._previous._line = _states._current._line;

            _states._current._line = _states._current._line switch
            {
                LineState.StartingLine => startingLine(@char),
                LineState.BeforeStartTag => beforeStartTag(@char),
                LineState.StartingTag => startingTag(@char),
                LineState.OpeningTag => openingTag(@char),
                LineState.ClosingTag => closingTag(@char),
                _ => throw new NotSupportedException($"{typeof(LineState).FullName} '{_states._current._line}' is not supported.")
            };
        }

        private readonly AfterStringIndicator _lineBreak = new(Environment.NewLine);
        private readonly AfterStringIndicator _startOpeningTag = new("<");
        private readonly AfterStringIndicator _endTag = new($">");
        private readonly AfterStringIndicator _startClosingTag = new("</");
        private readonly AfterStringIndicator _endSelfClosingTag = new($"/>");

        private LineState startingLine(char @char)
        {
            return Char.IsWhiteSpace(@char)
                ? LineState.StartingLine
                : beforeStartTag(@char);
        }

        private LineState beforeStartTag(char @char)
        {
            bool startingLine = _lineBreak.Indicate(@char);
            bool startOpeningTag = _startOpeningTag.Indicate(@char);
            bool startClosingTag = _startClosingTag.Indicate(@char);

            return startingLine
                ? LineState.StartingLine
                    : startOpeningTag
                        ? LineState.StartingTag
                        : LineState.BeforeStartTag;
        }

        private LineState startingTag(char @char)
        {
            return _startClosingTag.Indicate(@char)
                ? closingTag(@char)
                : openingTag(@char);
        }

        private LineState openingTag(char @char)
        {
            bool endTag = _endTag.Indicate(@char);

            return _endSelfClosingTag.Indicate(@char)
                ? beforeStartTag(@char)
                : endTag
                    ? beforeStartTag(@char)
                    : LineState.OpeningTag;
        }

        private LineState closingTag(char @char)
        {
            return _endTag.Indicate(@char)
                ? beforeStartTag(@char)
                : LineState.ClosingTag;
        }

        #endregion updateLineState.

        #endregion LineState.

        #endregion (Html)State.

        #endregion (Html)States.

    }
}
