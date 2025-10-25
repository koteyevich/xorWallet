using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Callbacks;

public class Pay : ICallback
{
    public Pay(Get get, ITelegramBotClient bot, IUserService users, IInvoiceService invoices)
    {
        _get = get;
        _bot = bot;
        _users = users;
        _invoices = invoices;
    }

    public string Name => "pay";

    private readonly Get _get;
    private readonly ITelegramBotClient _bot;
    private readonly IUserService _users;
    private readonly IInvoiceService _invoices;

    public async Task ExecuteAsync(CallbackQuery callbackQuery)
    {
        var id = string.Join('_', callbackQuery.Data.Split('_').Skip(1).ToArray());
        var invoice = await _get.Invoice(id);
        if (invoice == null)
        {
            await _bot.EditMessageText(callbackQuery.Message!.Chat.Id, callbackQuery.Message.Id,
                "Invoice does not exist!", replyMarkup: InlineKeyboardMarkup.Empty());
            return;
        }

        var invoiceOwner = await _get.User(invoice.OwnerUserId);
        var invoicePayer = await _get.User(callbackQuery.From.Id);

        invoicePayer.Balance -= invoice.XORs;
        invoiceOwner.Balance += invoice.XORs;

        if (invoicePayer.Balance < 0)
        {
            await _bot.EditMessageText(callbackQuery.Message!.Chat.Id, callbackQuery.Message.Id,
                "User balance cannot be negative. Insufficient balance.", replyMarkup: InlineKeyboardMarkup.Empty());
            return;
        }

        await _users.Edit(invoicePayer);
        await _users.Edit(invoiceOwner);
        await _invoices.Delete(invoice);


        await _bot.EditMessageText(callbackQuery.Message!.Chat.Id, callbackQuery.Message.Id,
            "Cha-ching!\n" +
            $"Your new balance is {invoicePayer.Balance} XOR (-{invoice.XORs})",
            replyMarkup: InlineKeyboardMarkup.Empty());
        await _bot.SendMessage(invoiceOwner.UserId, "Someone has paid your invoice!\n" +
                                                    $"Your new balance is {invoiceOwner.Balance} XOR (+{invoice.XORs})");
    }
}