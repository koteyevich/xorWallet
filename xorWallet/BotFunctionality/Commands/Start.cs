using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;

namespace xorWallet.BotFunctionality.Commands;

public class Start(ITelegramBotClient bot) : ICommand
{
    public BotCommand BotCommand => new("/start", "Start a bot.");

    public string[] Aliases => [];
    public int Order => 1;

    public async Task ExecuteAsync(Message message)
    {
        await bot.SendMessage(message, "Hello!");
    }
}