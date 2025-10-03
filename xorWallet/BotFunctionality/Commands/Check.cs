using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Commands;

public class Check : ICommand
{
    public Check(ITelegramBotClient bot, IUserService users, ICheckService checks, Get get,
        StartUrlGenerator urlGenerator)
    {
        _bot = bot;
        _users = users;
        _checks = checks;
        _get = get;
        _urlGenerator = urlGenerator;
    }

    public BotCommand BotCommand => new("/check", "Create a check.");
    public string[] Aliases => [];
    public int Order => 4;

    private readonly ITelegramBotClient _bot;
    private readonly IUserService _users;
    private readonly ICheckService _checks;
    private readonly Get _get;
    private readonly StartUrlGenerator _urlGenerator;

    public async Task ExecuteAsync(Message message)
    {
        var args = Parser.ParseArguments(message.Text!);

        if (args is { Length: < 2 })
        {
            await _bot.SendMessage(message, $"Not enough arguments. Example: /check 10 (XOR) 4 (activation amount)");
            return;
        }

        var userId = message.From!.Id;
        var xors = long.Parse(args[0]);
        var activationAmount = long.Parse(args[1]);

        if (activationAmount <= 0)
        {
            await _bot.SendMessage(message, "Activation amount must be greater than zero.");
            return;
        }

        if (xors <= 0)
        {
            await _bot.SendMessage(message, "XOR amount must be greater than zero.");
            return;
        }

        var user = await _get.User(message.From.Id);
        user.Balance -= xors * activationAmount;
        if (user.Balance < 0)
        {
            await _bot.SendMessage(message, "User balance cannot be negative. Insufficient balance.");
            return;
        }

        var check = new CheckModel
        {
            Activations = activationAmount,
            XORs = xors,
            OwnerUserId = userId
        };
        await _checks.Add(check);

        await _users.Edit(user);

        var keyboard = new InlineKeyboardMarkup().AddButton("🔳 QR", $"qr_{check.CheckId}");

        var botMessage = $"✅ Done! \n" +
                         $"➡ Share with this link to activate: <code>{_urlGenerator.Generate(check.CheckId)}</code> \n" +
                         $"💰 Your new balance is: <b>{user.Balance} XOR</b> <i>(- {xors * activationAmount})</i>";
        await _bot.SendMessage(message, botMessage, parseMode: ParseMode.Html, replyMarkup: keyboard);
    }
}