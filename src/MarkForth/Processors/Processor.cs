using MarkForth.Components;
using MarkForth.Extensions.IO;
using MarkForth.Extensions.Logging;
using MarkForth.Html;
using MarkForth.Input;
using MarkForth.IO;
using MarkForth.Scaffolds;
using Microsoft.Extensions.Logging;

namespace MarkForth.Processors;


// TODO: Formatting.

internal sealed class Processor : IProcessor
{

    #region Dependency Injection.

    private readonly IComponentService _componentService;
    private readonly IFile _file;
    private readonly ILogger<Processor> _logger;
    private readonly IScaffoldService _scaffoldService;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes an instance of the <see cref="Processor"/> class.
    /// </summary>
    public Processor(
        IComponentService componentService,
        IFile file,
        ILogger<Processor> logger,
        IScaffoldService scaffoldService,
        IServiceProvider serviceProvider
    )
    {
        _componentService = componentService;
        _file = file;
        _logger = logger;
        _scaffoldService = scaffoldService;
        _serviceProvider = serviceProvider;
    }

    #endregion Dependency Injection.

    #region IProcessor.

    public void Process(Stream input, Stream output)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);

        if (!input.CanRead)
            throw new ArgumentException("The input stream does not support reading.");

        if (!output.CanWrite)
            throw new ArgumentException("The output stream is not writable.");

        if (input.Length == 0)
            _logger.LogEmptyStream(nameof(input));

        Component[] components = input.Length == 0
            ? Enumerable.Empty<Component>().ToArray()
            : _componentService.Get(input).ToArray();

        using Scaffold? scaffold = this._scaffoldService.Get();

        using HtmlDocumentWriter htmlDocumentWriter = HtmlDocumentWriter.Create(_serviceProvider, output);
        HtmlElementWriter htmlElementWriter = htmlDocumentWriter.WriteHtmlStart();
        writeHead(htmlElementWriter, scaffold, components);
        writeBody(htmlElementWriter, input);
        htmlElementWriter.WriteHtmlEnd();
    }

    #region Head.

    private static bool shouldWriteHead(Scaffold? scaffold, IEnumerable<Component> components)
    {
        return shouldWriteHeadScript(scaffold, components)
            || shouldWriteHeadStyle(scaffold);
    }

    private static HtmlElementWriter writeHead(
        HtmlElementWriter htmlElementWriter,
        Scaffold? scaffold,
        IEnumerable<Component> components
    )
    {
        if (!shouldWriteHead(scaffold, components))
            return htmlElementWriter;

        HeadElementWriter headElementWriter = htmlElementWriter.WriteHeadStart();

        writeHeadScript(headElementWriter, scaffold, components);
        writeHeadStyle(headElementWriter, scaffold);

        foreach (Component component in components)
        {
            using (component)
            {
                TemplateElementWriter templateElementWriter = headElementWriter.WriteTemplateStart(component.Name);
                component.Style?.CopyTo(templateElementWriter.WriteStyleStart()).WriteStyleEnd();
                component.Template.CopyTo(templateElementWriter).WriteTemplateEnd();
            }
        }

        return headElementWriter.WriteHeadEnd();
    }

    #region Script.

    private static bool shouldWriteHeadScript(Scaffold? scaffold, IEnumerable<Component> components)
    {
        return scaffold?.Script != null
            || components.Any(component => component.Script != null);
    }

    private static HeadElementWriter writeHeadScript(
        HeadElementWriter headElementWriter,
        Scaffold? scaffold,
        IEnumerable<Component> components
    )
    {
        if (!shouldWriteHeadScript(scaffold, components))
            return headElementWriter;

        ScriptElementWriter scriptElementWriter = headElementWriter.WriteScriptStart();

        scaffold?.Script?.CopyTo(scriptElementWriter);

        foreach (Component component in components)
            component.Script?.CopyTo(scriptElementWriter);

        return scriptElementWriter.WriteScriptEnd();
    }

    #endregion Script.

    #region Style.

    private static bool shouldWriteHeadStyle(Scaffold? scaffold)
    {
        return scaffold?.Style != null;
    }

    private static HeadElementWriter writeHeadStyle(
        HeadElementWriter headElementWriter,
        Scaffold? scaffold
    )
    {
        if (shouldWriteHeadStyle(scaffold))
        {
            scaffold?.Style
                ?.CopyTo(headElementWriter.WriteStyleStart())
                ?.WriteStyleEnd();
        }

        return headElementWriter;
    }

    #endregion Style.

    #endregion Head.

    #region Body.

    private static bool shouldWriteBody(Stream input)
    {
        return input.Length > 0;
    }

    private HtmlElementWriter writeBody(
        HtmlElementWriter htmlElementWriter,
        Stream input
    )
    {
        input.Position = 0;

        using TextReader inputReader = InputReader.Create(_serviceProvider, input);

        return inputReader
            .CopyTo(htmlElementWriter.WriteBodyStart())
            .CloseBodyElement();
    }

    #endregion Body.

    #endregion IProcessor.

}
