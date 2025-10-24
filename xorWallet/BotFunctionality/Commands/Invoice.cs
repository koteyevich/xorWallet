using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Commands;

public class Invoice : ICommand
{
    public Invoice(ITelegramBotClient bot, IInvoiceService invoices, Get get,
        StartUrlGenerator urlGenerator)
    {
        _bot = bot;
        _invoices = invoices;
        _get = get;
        _urlGenerator = urlGenerator;
    }

    public BotCommand BotCommand => new("/invoice", "Create a new invoice.");
    public string[] Aliases => [];
    public int Order => 6;

    private readonly ITelegramBotClient _bot;
    private readonly IInvoiceService _invoices;
    private readonly Get _get;
    private readonly StartUrlGenerator _urlGenerator;

    public async Task ExecuteAsync(Message message)
    {
        var args = Parser.ParseCommandArguments(message.Text!);
        if (args is { Length: < 1 })
        {
            await _bot.SendMessage(message, $"Not enough arguments. Example: /invoice 10 (XOR)");
            return;
        }

        var userId = message.From!.Id;
        var xors = long.Parse(args[0]);

        if (xors <= 0)
        {
            await _bot.SendMessage(message, "XOR amount must be greater than zero.");
            return;
        }

        var invoice = new InvoiceModel()
        {
            XORs = xors,
            OwnerUserId = userId
        };
        await _invoices.Add(invoice);

        var keyboard = new InlineKeyboardMarkup().AddButton("🔳 QR", $"qr_{invoice.InvoiceId}");

        var botMessage = $"✅ Done! \n" +
                         $"➡ Share with this link to activate: <code>{_urlGenerator.Generate(invoice.InvoiceId)}</code>";
        await _bot.SendMessage(message, botMessage, parseMode: ParseMode.Html, replyMarkup: keyboard);
    }
}