using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;

namespace xorWallet.BotFunctionality.Callbacks;

public class Pay : ICallback
{
    public string Name => "pay";

    public async Task ExecuteAsync(CallbackQuery callbackQuery)
    {
    }
}