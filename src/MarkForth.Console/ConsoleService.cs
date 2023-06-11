using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MarkForth.Extensions;
using MarkForth.Extensions.Logging;
using MarkForth.Processors;
using MarkForth.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarkForth.Console;

internal sealed class ConsoleService : IHostedService
{

    #region Dependency Injection.

    private readonly IConfiguration configuration;
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly ILogger<ConsoleService> logger;
    private readonly IProcessor processor;

    public ConsoleService(
        IConfiguration configuration,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<ConsoleService> logger,
        IProcessor processor
    )
    {
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.configuration = configuration;
        this.logger = logger;
        this.processor = processor;
    }

    #endregion Dependency Injection.

    #region IHostedService.

    public Task StartAsync(CancellationToken cancellationToken)
    {
        const string inputName = "input";

        this.hostApplicationLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(() =>
            {
                string? inputPath = this.configuration[inputName];
                if (inputPath == null)
                {
                    error(new ArgumentException(inputName), "Required argument missing: --input [InputFilePath]");
                }
                else if (!File.Exists(inputPath))
                {
                    error(new ArgumentException(inputName), $"Input file not found: {inputPath}");
                }
                else
                {
                    try
                    {
                        this.processor.ProcessFileToFile(inputPath, "Output.html");
                    }
                    catch (XmlException exception)
                    {
                        error(exception, $"Input file does not contain valid XML: {inputPath}");
                    }
                    catch (StorageException exception)
                    {
                        error(exception, "The was a problem interacting with one of the configured storage systems.");
                    }
                    catch (InvalidOperationException exception)
                    {
                        error(exception, "More than one usable Scaffolds were found.");
                    }
                }

                this.hostApplicationLifetime.StopApplication();
            });
        });

        return Task.CompletedTask;
    }

    private void error(Exception exception, string message)
    {
        System.Console.Error.WriteLine();
        System.Console.Error.WriteLine(message);
        System.Console.Error.WriteLine($"More info in log entry: {this.logger.LogError(exception)}");
        System.Console.Error.WriteLine();
        Environment.ExitCode = exception.HResult;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    #endregion IHostedService.

}
