using Telegram.Bot;
using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Processors;
using xorWallet.Interfaces;

namespace xorWallet.BotFunctionality.Commands
{
    public class MyChecks : CommandBase
    {
        public override string Name => "/mychecks";

        public override string Description => "Лист чеков, которые вы создали.";

        public override string[] Aliases => [];

        protected override async Task ExecuteCoreAsync(Message message, TelegramBotClient bot)
        {
            await MyChecksProcessor.MyChecksAsync(message, bot);
        }
    }
}
