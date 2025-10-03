using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Services;

namespace xorWallet.BotFunctionality.Commands;

public class Help(ITelegramBotClient bot) : ICommand
{
    public BotCommand BotCommand => new("/help", "About the bot and the list of all commands.");

    public string[] Aliases => [];
    public int Order => 2;

    public async Task ExecuteAsync(Message message)
    {
        var helpMessage = "<b>Available commands:</b>\n\n";
        var listed = new HashSet<ICommand>();

        foreach (var command in CommandRegistryService.CommandsReadOnly.Values.Distinct())
        {
            if (!listed.Add(command))
                continue;

            var aliasText = command.Aliases.Length > 0
                ? $" ({string.Join(", ", command.Aliases.Select(a => $"{a}"))})"
                : "";

            helpMessage += $"{command.BotCommand.Command}{aliasText} - {command.BotCommand.Description}\n";
        }

        await bot.SendMessage(message, helpMessage,
            parseMode: ParseMode.Html,
            linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
    }
}