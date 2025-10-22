using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Services.Interfaces;
using xorWallet.Settings;

namespace xorWallet.Services;

public class CommandRegistryService : ICommandRegistryService
{
    private static readonly Dictionary<string, ICommand> Commands = new();
    private readonly ILogger<CommandRegistryService> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IBotInfo _botInfo;

    public CommandRegistryService(IEnumerable<ICommand> commands,
        ILogger<CommandRegistryService> logger, IBotInfo botInfo, ITelegramBotClient bot)
    {
        _logger = logger;
        _botInfo = botInfo;
        _bot = bot;

        var sortedCommands = commands.OrderBy(c => c.Order).ToList();
        var botCommandsList = new List<BotCommand>();

        foreach (var command in sortedCommands)
        {
            var commandName = command.BotCommand.Command.ToLower();
            CommandRegistryService.Commands[commandName] = command;

            botCommandsList.Add(command.BotCommand);

            foreach (var alias in command.Aliases)
            {
                Commands[alias.ToLower()] = command;
            }
        }

        _bot.SetMyCommands(botCommandsList);
    }

    public async Task HandleCommand(Message message)
    {
        var commandParts = message.Text!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var commandText = commandParts[0].ToLowerInvariant();
        var commandSplit = commandText.Split('@', StringSplitOptions.RemoveEmptyEntries);
        var commandName = commandSplit[0];
        _logger.LogDebug("Command name is: {commandName}", commandName);
        var targetBot = commandSplit.Length > 1 ? commandSplit[1] : null;
        _logger.LogDebug("Target bot is: {targetBot}", targetBot);

        if (targetBot != null && !string.Equals(targetBot, _botInfo.Me.Username, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (Commands.TryGetValue(commandName, out var command))
        {
            _logger.LogInformation("Executing command: {CommandName}", commandName);
            await command.ExecuteAsync(message);
        }
    }

    public static ReadOnlyDictionary<string, ICommand> CommandsReadOnly => Commands.AsReadOnly();
}