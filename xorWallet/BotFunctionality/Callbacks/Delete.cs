using Telegram.Bot;
using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Callbacks;

public class Delete : ICallback
{
    public Delete(Get get, ITelegramBotClient bot, IUserService users, IInvoiceService invoices, ICheckService checks)
    {
        _get = get;
        _bot = bot;
        _users = users;
        _invoices = invoices;
        _checks = checks;
    }

    public string Name => "delete";

    private readonly Get _get;
    private readonly ICheckService _checks;
    private readonly IInvoiceService _invoices;
    private readonly IUserService _users;
    private readonly ITelegramBotClient _bot;

    public async Task ExecuteAsync(CallbackQuery callbackQuery)
    {
        var user = await _get.User(callbackQuery.From.Id);

        await _checks.DeleteAllOfUser(user);
        await _invoices.DeleteAllOfUser(user);
        await _users.Delete(user);

        await _bot.EditMessageText(callbackQuery.Message.Chat.Id,
            callbackQuery.Message.Id, "Everything has been deleted.\n" +
                                      "Goodbye!");
    }
}