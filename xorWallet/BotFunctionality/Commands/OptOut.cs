using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;

namespace xorWallet.BotFunctionality.Commands;

public class OptOut : ICommand
{
    public BotCommand BotCommand => new("/opt_out", "Remove your data from the bot.");
    public string[] Aliases => ["optout"];
    public int Order => 999;

    private readonly ITelegramBotClient _bot;
    private readonly Get _get;

    public OptOut(ITelegramBotClient bot, Get get)
    {
        _bot = bot;
        _get = get;
    }

    public async Task ExecuteAsync(Message message)
    {
        //TODO: yes/no callbacks
        var user = await _get.User(message.From!.Id);
        var keyboard = new InlineKeyboardMarkup().AddButton("Yes", "delete").AddButton("No", "decline");
        await _bot.SendMessage(message, $"Are you sure you want to remove your data from the bot? \n" +
                                        $"<b>ALL data will be removed, checks and invoices that your created, and your balance!</b> \n" +
                                        $"Here's data about you that is currently stored: \n" +
                                        $"\n" +
                                        $"Your Database ID: <tg-spoiler><b>{user.Id}</b></tg-spoiler>\n" +
                                        $"Your Telegram UserID: <b>{user.UserId}</b>\n" +
                                        $"Your Balance: <b>{user.Balance} XORs</b>\n" +
                                        $"\n" +
                                        $"<i>To find information about your checks and invoices, run commands /my_checks and /my_invoices </i>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html);
    }
}