using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Commands.StartFunctions;

public class Invoice : IStartFunction
{
    public Invoice(ITelegramBotClient bot, Get get, IUserService users, IInvoiceService checks,
        StartUrlGenerator urlGenerator, ILogger<Invoice> logger)
    {
        _bot = bot;
        _get = get;
        _users = users;
        _checks = checks;
        _urlGenerator = urlGenerator;
        _logger = logger;
    }

    public string Name => $"{(int)Bank.Invoice}";

    private readonly ITelegramBotClient _bot;
    private readonly Get _get;
    private readonly IUserService _users;
    private readonly IInvoiceService _checks;
    private readonly ILogger<Invoice> _logger;
    private readonly StartUrlGenerator _urlGenerator;


    public async Task ExecuteAsync(Message message, string[] data)
    {
        var id = string.Join('_', data);
        var invoice = await _get.Invoice(id);

        if (invoice == null)
        {
            await _bot.SendMessage(message, "Invoice not found.");
            return;
        }

        InlineKeyboardMarkup keyboard;

        if (invoice.OwnerUserId == message.From!.Id)
        {
            var url = _urlGenerator.Generate(invoice.InvoiceId);

            keyboard = new InlineKeyboardMarkup().AddButton("⬅ Revoke Check", $"revoke_{invoice.InvoiceId}")
                .AddButton("🔲 QR", $"qr_{invoice.InvoiceId}");
            await _bot.SendMessage(message, $"📋 This is your check. \n" +
                                            $"XORs waiting to be paid: <b>{invoice.XORs}</b>\n" +
                                            $"\n" +
                                            $"Share the invoice with someone:\n" +
                                            $"<code>{url}</code>", parseMode: ParseMode.Html, replyMarkup: keyboard);
            return;
        }

        keyboard = new InlineKeyboardMarkup()
            .AddButton("Pay", $"pay_{invoice.InvoiceId}")
            .AddButton("Decline", "decline");
        await _bot.SendMessage(message, $"An invoice for {invoice.XORs} XOR.", replyMarkup: keyboard);
    }
}