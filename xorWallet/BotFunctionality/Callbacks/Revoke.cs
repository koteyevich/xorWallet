using Telegram.Bot;
using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Callbacks;

public class Revoke : ICallback
{
    public Revoke(ITelegramBotClient bot, Get get, IInvoiceService invoices, ICheckService checks, IUserService users)
    {
        _bot = bot;
        _get = get;
        _invoices = invoices;
        _checks = checks;
        _users = users;
    }

    public string Name => "revoke";

    private readonly ITelegramBotClient _bot;
    private readonly Get _get;
    private readonly IUserService _users;
    private readonly ICheckService _checks;
    private readonly IInvoiceService _invoices;

    public async Task ExecuteAsync(CallbackQuery callbackQuery)
    {
        var data = Parser.ParseArguments(callbackQuery.Data!, '_').Skip(1).ToArray();

        if (!Enum.TryParse<Bank>(data.First(), out var bankType))
            return;

        switch (bankType)
        {
            case Bank.Check:
                var check = await _get.Check(string.Join('_', data));
                if (check == null)
                {
                    await _bot.AnswerCallbackQuery(callbackQuery.Id, "Check does not exist!", true);
                    return;
                }

                var owner = await _get.User(callbackQuery.From.Id);
                owner.Balance += check.XORs * check.Activations;

                await _users.Edit(owner);
                await _checks.Delete(check);
                await _bot.AnswerCallbackQuery(callbackQuery.Id, "Check successfully revoked!", true);
                break;
            case Bank.Invoice:
                var invoice = await _get.Invoice(string.Join('_', data));
                if (invoice == null)
                {
                    await _bot.AnswerCallbackQuery(callbackQuery.Id, "Invoice does not exist!", true);
                    return;
                }

                await _invoices.Delete(invoice);
                await _bot.AnswerCallbackQuery(callbackQuery.Id, "Invoice successfully revoked!", true);
                break;
        }
    }
}