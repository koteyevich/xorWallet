using Telegram.Bot.Types;

namespace xorWallet.BotFunctionality.Interfaces;

public interface ICommand
{
    BotCommand BotCommand { get; }
    string[] Aliases { get; }
    int Order { get; }
    Task ExecuteAsync(Message message);
}