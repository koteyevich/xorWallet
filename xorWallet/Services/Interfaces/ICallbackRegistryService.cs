using Telegram.Bot.Types;

namespace xorWallet.Services.Interfaces;

public interface ICallbackRegistryService
{
    Task HandleCallbackAsync(CallbackQuery callbackQuery);
}