using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;

namespace xorWallet.BotFunctionality.Commands;

public class OptOut : ICommand
{
    public BotCommand BotCommand => new("/opt_out", "Remove your data from the bot.");

    public string[] Aliases => ["optout"];
    public int Order => 999;

    public async Task ExecuteAsync(Message message)
    {
    }
}