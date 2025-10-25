using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;

namespace xorWallet.BotFunctionality.Callbacks;

public class Decline : ICallback
{
    public string Name => "decline";

    private readonly ITelegramBotClient _bot;

    public Decline(ITelegramBotClient bot)
    {
        _bot = bot;
    }

    public async Task ExecuteAsync(CallbackQuery callbackQuery)
    {
        await _bot.EditMessageText(callbackQuery.Message!.Chat.Id, callbackQuery.Message.Id, "Declined.",
            replyMarkup: InlineKeyboardMarkup.Empty());
    }
}