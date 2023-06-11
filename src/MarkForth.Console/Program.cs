using MarkForth.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace MarkForth.Console;

internal sealed class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder hostBuilder = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(services => services.AddHostedService<ConsoleService>().AddMarkForth())
            .UseConsoleLifetime()
            .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);

        using IHost host = hostBuilder.Build();

        await host.RunAsync();
    }
}
