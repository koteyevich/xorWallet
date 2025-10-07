using Telegram.Bot.Types;

namespace xorWallet.BotFunctionality.Interfaces;

public interface IStartFunction
{
    string Name { get; }

    Task ExecuteAsync(Message message, string[] data);
}