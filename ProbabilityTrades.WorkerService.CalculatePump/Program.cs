var host = Host.CreateDefaultBuilder(args)
.ConfigureServices()
.ConfigureLogging()
.Build();

host.ConfigureScheduler();

await host.RunAsync();