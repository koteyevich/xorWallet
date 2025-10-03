using Telegram.Bot;
using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;

namespace xorWallet.BotFunctionality.Commands;

public class Balance : ICommand
{
    public Balance(ITelegramBotClient bot, Get get)
    {
        _bot = bot;
        _get = get;
    }

    public BotCommand BotCommand => new("/balance", "Your current balance.");
    public string[] Aliases => [];
    public int Order => 3;

    private readonly ITelegramBotClient _bot;
    private readonly Get _get;

    public async Task ExecuteAsync(Message message)
    {
        var user = await _get.User(message.From!.Id);
        await _bot.SendMessage(message, $"Your balance is {user.Balance}");
    }
}