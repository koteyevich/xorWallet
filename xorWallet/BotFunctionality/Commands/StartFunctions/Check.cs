using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Services.Interfaces;

namespace xorWallet.BotFunctionality.Commands.StartFunctions;

public class Check : IStartFunction
{
    public string Name => "check";

    private readonly ITelegramBotClient _bot;
    private readonly Get _get;
    private readonly IUserService _users;
    private readonly ICheckService _checks;
    private readonly StartUrlGenerator _urlGenerator;

    public Check(Get get, ITelegramBotClient bot, ICheckService checks, IUserService users,
        StartUrlGenerator urlGenerator)
    {
        _get = get;
        _bot = bot;
        _checks = checks;
        _users = users;
        _urlGenerator = urlGenerator;
    }

    public async Task ExecuteAsync(Message message, string[] data)
    {
        var id = string.Join('_', data);
        var check = await _get.Check(id);

        if (check == null)
        {
            await _bot.SendMessage(message, "Check not found.");
            return;
        }

        if (check.OwnerUserId == message.From!.Id)
        {
            var url = _urlGenerator.Generate(check.CheckId);

            var keyboard = new InlineKeyboardMarkup().AddButton("⬅ Revoke Check", $"revoke_{check.CheckId}")
                .AddButton("🔲 QR", $"qr_{check.CheckId}");
            await _bot.SendMessage(message, $"📋 This is your check. \n" +
                                            $"Activations remaining: <b>{check.Activations}</b> \n" +
                                            $"XORs each activation: <b>{check.XORs}</b>\n" +
                                            $"If you revoke check now, you will receive <b>{check.XORs * check.Activations}</b> XORs back.\n" +
                                            $"\n" +
                                            $"If you don't want to revoke the check, you can share it:\n" +
                                            $"<code>{url}</code>", parseMode: ParseMode.Html, replyMarkup: keyboard);
            return;
        }

        var claimingUser = await _get.User(message.From.Id);

        claimingUser.Balance += check.XORs;

        check.Activations--;
        check.UsersActivated.Add(claimingUser);

        await _users.Edit(claimingUser);
        await _checks.Edit(check);

        await _bot.SendMessage(message, $"✅ Done!\n" +
                                        $"💰 New balance: {claimingUser.Balance} (+ {check.XORs})");
    }
}