using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Commands;

public class MyInvoices : ICommand
{
    public MyInvoices(ITelegramBotClient bot, Get get, IInvoiceService invoices, StartUrlGenerator urlGenerator)
    {
        _bot = bot;
        _get = get;
        _invoices = invoices;
        _urlGenerator = urlGenerator;
    }

    public BotCommand BotCommand => new("/my_invoices", "List all of your invoices.");
    public string[] Aliases => [];
    public int Order => 7;

    private readonly ITelegramBotClient _bot;
    private readonly Get _get;
    private readonly IInvoiceService _invoices;
    private readonly StartUrlGenerator _urlGenerator;

    public async Task ExecuteAsync(Message message)
    {
        var user = await _get.User(message.From!.Id);
        var allInvoices = _invoices.GetAllOfUser(user);

        if (allInvoices.Count <= 0)
        {
            await _bot.SendMessage(message, "You have no active invoices!");
            return;
        }

        var keyboard = new InlineKeyboardMarkup();

        foreach (var invoice in allInvoices)
        {
            keyboard.AddNewRow(
                InlineKeyboardButton.WithUrl(
                    $"Invoice: {invoice.XORs} XORs (ID: ...{invoice.InvoiceId[^5..]})",
                    _urlGenerator.Generate(invoice.InvoiceId)));
        }

        await _bot.SendMessage(message, "Here's your active invoices: ", replyMarkup: keyboard);
    }
}