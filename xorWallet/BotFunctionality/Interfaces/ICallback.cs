using Telegram.Bot.Types;

namespace xorWallet.BotFunctionality.Interfaces;

public interface ICallback
{
    string Name { get; }

    Task ExecuteAsync(CallbackQuery callbackQuery);
}