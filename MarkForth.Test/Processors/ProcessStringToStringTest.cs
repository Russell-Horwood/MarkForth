using System.Xml.Linq;
using MarkForth.Components;
using MarkForth.Extensions;
using MarkForth.Html;
using MarkForth.Scaffolds;
using Xunit.Abstractions;

namespace MarkForth.Test.Processors;

public class ProcessStringToString : ProcessorTest
{

    #region Dependency Injection.

    private readonly ITestOutputHelper _testOutputHelper;

    public ProcessStringToString(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #endregion Dependency Injection.

    #region Tests.

    [Fact]
    public async Task DefaultScaffoldScriptIsWrittenWhenNotSpecifiedAsync()
    {
        // Arrange.
        string scriptLine1 = Guid.NewGuid().ToString();
        string scriptLine2 = Guid.NewGuid().ToString();
        string script = $"{scriptLine1}{Environment.NewLine}{scriptLine2}{Environment.NewLine}";
        const string indent = "  ";

        using Scaffold _ = await this.ScaffoldService.InsertAsync(new Scaffold
        {
            Script = script.ToMemoryStream()
        });

        // Act.
        string output = act("");

        // Assert.
        Assert.Equal
        (
            $"\n{indent}{indent}{indent}{scriptLine1}\n{indent}{indent}{indent}{scriptLine2}\n{indent}{indent}",
            XDocument
                .Parse(output)
                .Descendants(ElementNames.Script)
                .SingleOrDefault()
                ?.Value
        );
    }

    [Fact]
    public void HeadElementIsNotWrittenWhenEmpty()
    {
        // Arrange.
        string input = "";

        // Act.
        string output = act(input);

        // Assert.
        Assert.DoesNotContain(ElementNames.Head, output);
    }

    [Fact]
    public void ImagesAreEmbeddedWhenLocal()
    {
        // Arrange.
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetRandomFileName());
        this.FileMock.Setup(file => file.Exists(imagePath)).Returns(true);
        this.FileMock.Setup(file => file.OpenRead(imagePath)).Returns(new MemoryStream(new byte[] { 0xFF, 0x00 }));

        // Act.
        string output = act($"<img src=\"{imagePath}\"/>");

        // Assert.
        Assert.Equal
        (
            $"data:image/png;base64, /wA=",
            XDocument
                .Parse(output)
                .Descendants(ElementNames.Img)
                .SingleOrDefault()
                ?.Attributes(AttributeNames.Src)
                ?.SingleOrDefault()
                ?.Value
        );
    }

    [Fact]
    public async Task LineBreaksAreWrittenAfterTagsAsync()
    {
        // Arrange.
        using Component component = await this.ComponentService.InsertAsync(new Component
        {
            Name = "a-component",
            Template = $"".ToMemoryStream()
        });
        using Scaffold _ = await this.ScaffoldService.InsertAsync(new Scaffold
        {
            Script = " ".ToMemoryStream(),
            Style = " ".ToMemoryStream()
        });

        // Act.
        string output = act($"<{component.Name}/>");

        // Assert.
        Assert.All
        (
            new[]
            {
                ElementNames.Html,
                ElementNames.Head,
                ElementNames.Script,
                ElementNames.Style,
                ElementNames.Body
            },
            elementName =>
            {
                Assert.Multiple
                (
                    () =>
                    {
                        string startElement = $"<{elementName}>";
                        Assert.StartsWith
                        (
                            Environment.NewLine,
                            output.Substring(output.IndexOf(startElement, StringComparison.Ordinal) + startElement.Length)
                        );
                    },
                    () =>
                    {
                        string endElement = $"</{elementName}>";
                        Assert.StartsWith
                        (
                            Environment.NewLine,
                            output.Substring(output.IndexOf(endElement, StringComparison.Ordinal) + endElement.Length)
                        );
                    }
                );
            }
        );
    }

    [Fact]
    public async Task TemplateIsWrittenWhenComponentIsReferencedAsync()
    {
        // Arrange.
        using Component component = await this.ComponentService.InsertAsync(new Component
        {
            Name = "a-component",
            Template = $"".ToMemoryStream()
        });
        using Scaffold _ = await this.ScaffoldService.InsertAsync(new Scaffold
        {
            Script = " ".ToMemoryStream(),
            Style = " ".ToMemoryStream()
        });

        // Act.
        string output = act($"<{component.Name}/>");

        // Assert.
        Assert.Equal
        (
            component.Name,
            XDocument
                .Parse(output)
                .Element(ElementNames.Html)
                ?.Element(ElementNames.Head)
                ?.Element(ElementNames.Template)
                ?.Attribute(AttributeNames.Id)
                ?.Value
        );
    }

    #region White Space.

    private static readonly string _whiteSpace = $"\t  {Environment.NewLine}  \t";

    [Fact]
    public void WhiteSpaceIsPreservedInPreElements()
    {
        // Arrange.
        string input = $"<pre>{_whiteSpace}</pre>";

        // Act.
        string output = act(input);

        // Assert.
        Assert.StartsWith
        (
            _whiteSpace,
            output.Substring(output.IndexOf("<pre>", StringComparison.Ordinal) + 5)
        );
    }

    #endregion White Space.

    private string act(string input)
    {
        string output = this.Processor.ProcessStringToString(input);
        _testOutputHelper.WriteLine(output);
        return output;
    }

    #endregion Tests.

}
