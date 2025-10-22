using Telegram.Bot;
using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;

namespace xorWallet.BotFunctionality.Commands;

public class Start : ICommand
{
    public BotCommand BotCommand => new("/start", "Start a bot.");
    public string[] Aliases => [];
    public int Order => 1;

    private readonly ITelegramBotClient _bot;
    private readonly Dictionary<string, IStartFunction> _functions = new();

    public Start(IEnumerable<IStartFunction> functions, ITelegramBotClient bot)
    {
        _bot = bot;

        foreach (var function in functions)
        {
            _functions[function.Name.ToLower()] = function;
        }
    }


    public async Task ExecuteAsync(Message message)
    {
        var arguments = Parser.ParseCommandArguments(message.Text!, '_');

        if (arguments.Length == 0)
        {
            await _bot.SendMessage(message, "Hello!");
        }
        else
        {
            var key = arguments[0].ToLower();
            if (_functions.TryGetValue(key, out var function))
            {
                await function.ExecuteAsync(message, arguments);
            }
        }
    }
}