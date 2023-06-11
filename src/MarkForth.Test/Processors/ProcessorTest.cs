using MarkForth.Components;
using MarkForth.Extensions.DependencyInjection;
using MarkForth.IO;
using MarkForth.Processors;
using MarkForth.Scaffolds;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MarkForth.Test.Processors;

public class ProcessorTest
{
    protected IComponentService ComponentService { get; }
    protected Mock<IFile> FileMock { get; } = new Mock<IFile>();
    protected IProcessor Processor { get; }
    protected IScaffoldService ScaffoldService { get; }

    protected ProcessorTest()
    {
        IServiceProvider serviceProvider = new ServiceCollection()
            .AddMarkForth(builder => builder.AddInMemoryStores())
            .AddSingleton(this.FileMock.Object)
            .BuildServiceProvider();

        this.ComponentService = serviceProvider.GetRequiredService<IComponentService>();
        this.Processor = serviceProvider.GetRequiredService<IProcessor>();
        this.ScaffoldService = serviceProvider.GetRequiredService<IScaffoldService>();
    }
}
