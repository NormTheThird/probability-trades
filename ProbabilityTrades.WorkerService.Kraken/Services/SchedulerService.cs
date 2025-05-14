namespace ProbabilityTrades.WorkerService.Kraken.Services;

public static class SchedulerService
{
    /// <summary>
    ///     Documentation: https://docs.coravel.net/Scheduler/
    ///     
    ///     Method Description:
    ///         EverySecond()           Run the task every second
    ///         EveryFiveSeconds()      Run the task every five seconds
    ///         EveryTenSeconds()       Run the task every ten seconds
    ///         EveryFifteenSeconds()   Run the task every fifteen seconds
    ///         EveryThirtySeconds()    Run the task every thirty seconds
    ///         EverySeconds(3)         Run the task every 3 seconds.
    ///         EveryMinute()           Run the task once a minute
    ///         EveryFiveMinutes()      Run the task every five minutes
    ///         EveryTenMinutes()       Run the task every ten minutes
    ///         EveryFifteenMinutes()   Run the task every fifteen minutes
    ///         EveryThirtyMinutes()    Run the task every thirty minutes
    ///         Hourly()                Run the task every hour
    ///         HourlyAt(12)            Run the task at 12 minutes past every hour
    ///         Daily()                 Run the task once a day at midnight
    ///         DailyAtHour(13)         Run the task once a day at 1 p.m.UTC
    ///         DailyAt(13, 30)         Run the task once a day at 1:30 p.m.UTC
    ///         Weekly()                Run the task once a week
    ///         Cron("* * * * *")       Run the task using a Cron expression
    ///         
    ///     Run At Startup: .RunOnceAtStart()
    /// </summary>
    /// <param name="host"></param>
    public static void ConfigureScheduler(this IHost host)
    {
        host.Services.UseScheduler(scheduler =>
        {
            scheduler.Schedule<FiveMinuteProcess>().EveryFiveMinutes().PreventOverlapping("FiveMinuteProcess");
            scheduler.Schedule<FifteenMinuteProcess>().EveryFifteenMinutes().PreventOverlapping("FifteenMinuteProcess");
            scheduler.Schedule<HourlyProcess>().Hourly().PreventOverlapping("HourlyProcess");
            scheduler.Schedule<DailyProcess>().DailyAtHour(1); // 1 a.m.UTC which is 8 p.m.CST DST or 7 p.m. CST Standard Time
        });
    }
}