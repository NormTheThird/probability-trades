
using DSharpPlus.Exceptions;

namespace ProbabilityTrades.Bot.Discord.Processes;

public class SubscriptionProcess : BaseProcess<SubscriptionProcess>, IInvocable
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionProcess(IConfiguration configuration, ILogger<SubscriptionProcess> logger, DiscordClient discordClient,
                               ISubscriptionService subscriptionService)
        : base(configuration, logger, discordClient)
    {
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
    }

    public async Task Invoke()
    {
        await SynchronizeUsersAndSubscriptionsAsync();

    }
    public async Task SynchronizeUsersAndSubscriptionsAsync()
    {
        var guild = await _discordClient.GetGuildAsync(_configuration.GetValue<ulong>("Discord:ServerId"));
        var everyoneRole = guild.EveryoneRole;
        var currentMembers = await guild.GetAllMembersAsync();
        var ptMemberRole = guild.Roles.FirstOrDefault(_ => _.Key.Equals(_configuration.GetValue<ulong>("Discord:PTMemberRoleId")));
        var verifiedRole = guild.Roles.FirstOrDefault(_ => _.Key.Equals(_configuration.GetValue<ulong>("Discord:VerifiedRoleId")));

        var usersAndSubscriptions = await _subscriptionService.GetUsersAndSubscriptionsAsync();
        foreach (var user in usersAndSubscriptions)
        {
            if (user.DiscordUserId.Equals(0))
            {
                _logger.LogWarning($"User {user.Username} does not have a discord user id.");
                continue;
            }

            if (string.IsNullOrEmpty(user.DiscordAccessToken))
            {
                _logger.LogWarning($"User {user.Username} does not have a discord access token.");
                continue;
            }

            try
            {
                var currentMember = currentMembers.FirstOrDefault(_ => _.Id.Equals((ulong)user.DiscordUserId));
                if (currentMember is null)
                {
                    if (user.Status.Equals("Deleted"))
                        continue;

                    if (user.Status.Equals("PT-Member") || user.Status.Equals("Verified"))
                    {
                        var accessToken = await user.DiscordAccessToken.DecryptAsync(_configuration.GetValue<string>("EncryptionKey"));
                        var discordUser = await _discordClient.GetUserAsync((ulong)user.DiscordUserId);
                        await guild.AddMemberAsync(discordUser, accessToken);
                    }
                }
                else
                {
                    var isVeriified = currentMember.Roles.Any(_ => _.Name.Equals("Verified"));
                    var isPTMember = currentMember.Roles.Any(_ => _.Name.Equals("PT-Member"));
                    if (user.Status.Equals("Deleted"))
                        await currentMember.RemoveAsync();
                    else if (user.Status.Equals("PT-Member") && !isPTMember)
                        // If user is PT-Member and not a PT-Member on the server then add them to the PT-Member role
                        await currentMember.GrantRoleAsync(ptMemberRole.Value);
                    else if (!user.Status.Equals("PT-Member") && isPTMember)
                        // If user is not a PT-Member and is a PT-Member on the server then remove them from the PT-Member role
                        await currentMember.RevokeRoleAsync(ptMemberRole.Value);
                    else
                        _logger.LogInformation("Skipping {discordUserId} : {discordUsername} : {status}", user.DiscordUserId, user.Username, user.Status);
                }
            }
            catch (Exception ex)
            {
                var jsonMessage = "";
                if(ex is UnauthorizedException) 
                    jsonMessage = (ex as UnauthorizedException).JsonMessage;
                _logger.LogError(ex, "Error updating user {discordUsername} to server. {jsonMessage}", user.Username, jsonMessage);
            }
        }
    }

    private static async Task<(bool IsMember, List<string> Roles)> GetServerMemberAsync(DiscordGuild guild, ulong discordUserId)
    {
        try
        {
            var serverMember = await guild.GetMemberAsync(discordUserId);
            return (true, serverMember.Roles.Select(x => x.Name).ToList());
        }
        catch (Exception)
        {
            return (false, new());
        }
    }
}