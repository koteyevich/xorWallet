using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Services.Interfaces;

namespace xorWallet.Services;

public class CallbackRegistryService : ICallbackRegistryService
{
    private readonly Dictionary<string, ICallback> _callbacks = new();
    private readonly ILogger<CallbackRegistryService> _logger;
    private readonly ITelegramBotClient _bot;

    public CallbackRegistryService(IEnumerable<ICallback> callbacks, ILogger<CallbackRegistryService> logger,
        ITelegramBotClient bot)
    {
        _logger = logger;
        _bot = bot;

        foreach (var callback in callbacks)
        {
            _callbacks[callback.Name.ToLower()] = callback;
        }
    }

    public async Task HandleCallbackAsync(CallbackQuery callbackQuery)
    {
        if (string.IsNullOrWhiteSpace(callbackQuery.Data)) return;

        var key = callbackQuery.Data.Split('_')[0].ToLower();
        _logger.LogDebug("Query data key: {key}", key);

        if (_callbacks.TryGetValue(key, out var callback))
        {
            _logger.LogInformation("Handling callback: {callbackQuery.Data}", callbackQuery.Data);
            await callback.ExecuteAsync(callbackQuery);
        }
        else
        {
            await _bot.AnswerCallbackQuery(callbackQuery.Id, "Unknown button...");
        }
    }
}