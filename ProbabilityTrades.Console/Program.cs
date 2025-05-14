using ProbabilityTrades.Console;

var host = Host.CreateDefaultBuilder(args)
.ConfigureServices()
.ConfigureLogging()
.Build();

var svc = ActivatorUtilities.CreateInstance<Worker>(host.Services);
await svc.RunAsync();