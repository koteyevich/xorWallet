using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Commands;

public class MyChecks : ICommand
{
    public BotCommand BotCommand => new("/my_checks", "List all of your checks.");
    public string[] Aliases => [];
    public int Order => 5;

    private readonly ITelegramBotClient _bot;
    private readonly Get _get;
    private readonly ICheckService _checks;
    private readonly StartUrlGenerator _urlGenerator;

    public MyChecks(ITelegramBotClient bot, Get get, ICheckService checks,
        StartUrlGenerator urlGenerator)
    {
        _bot = bot;
        _get = get;
        _checks = checks;
        _urlGenerator = urlGenerator;
    }

    public async Task ExecuteAsync(Message message)
    {
        var user = await _get.User(message.From!.Id);
        var allChecks = _checks.GetAllOfUser(user);

        if (allChecks.Count <= 0)
        {
            await _bot.SendMessage(message, "You have no active checks!");
            return;
        }

        var keyboard = new InlineKeyboardMarkup();

        foreach (var check in allChecks)
        {
            keyboard.AddNewRow(
                InlineKeyboardButton.WithUrl(
                    $"Check: {check.XORs} XORs (ID: ...{check.CheckId[^5..]})",
                    _urlGenerator.Generate(check.CheckId)));
        }

        await _bot.SendMessage(message, "Here's your active checks: ", replyMarkup: keyboard);
    }
}