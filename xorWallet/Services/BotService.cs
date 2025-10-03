using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using xorWallet.Helpers;
using xorWallet.Services.Interfaces;
using xorWallet.Settings;

namespace xorWallet.Services;

public class BotService : BackgroundService
{
    private readonly ILogger<BotService> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBotInfo _botInfo;

    public BotService(ILogger<BotService> logger, ITelegramBotClient bot, IServiceScopeFactory scopeFactory,
        IBotInfo botInfo)
    {
        _logger = logger;
        _bot = bot;
        _scopeFactory = scopeFactory;
        _botInfo = botInfo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _bot.StartReceiving(
            receiverOptions: new ReceiverOptions { AllowedUpdates = [] },
            updateHandler: OnUpdate,
            errorHandler: OnError,
            cancellationToken: stoppingToken
        );

        var me = await _bot.GetMe(stoppingToken);
        _logger.LogInformation("Bot connected as {me.Username}", me.Username);
        _botInfo.Me = me;

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        // i hate this...
        using var scope = _scopeFactory.CreateScope();
        var commandRegistry = scope.ServiceProvider.GetRequiredService<ICommandRegistryService>();
        var callbackRegistry = scope.ServiceProvider.GetRequiredService<ICallbackRegistryService>();

        switch (update)
        {
            case { Message: not null }:
                var message = update.Message;
                try
                {
                    if (message.Text != null && message.Text.StartsWith('/'))
                    {
                        await commandRegistry.HandleCommand(message);
                    }
                }
                catch (Exception e)
                {
                    // you might think: "but why not use OnError"? good question.
                    // well, OnError is used, but it is only used to report the issue to sentry and to logs.
                    // why only use it for that? because in there i can't pass additional arguments that would-
                    // allow me to notify user that something is wrong!
                    await _bot.SendMessage(message, $"<b>Ah!</b> <i>Something happened...</i> \n" +
                                                    $"<blockquote>{e.Message}</blockquote>",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);
                    throw;
                }

                break;
            case { CallbackQuery: not null }:
                var callback = update.CallbackQuery;
                try
                {
                    await callbackRegistry.HandleCallbackAsync(callback);
                }
                catch (Exception e)
                {
                    await _bot.AnswerCallbackQuery(callback.Id, "Something happened... \n" +
                                                                $"{e.Message}", true,
                        cancellationToken: cancellationToken);
                    throw;
                }

                break;
        }
    }

    private async Task OnError(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, exception.Message);
    }
}