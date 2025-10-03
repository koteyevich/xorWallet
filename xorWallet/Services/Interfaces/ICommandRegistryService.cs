using Telegram.Bot.Types;

namespace xorWallet.Services.Interfaces;

public interface ICommandRegistryService
{
    Task HandleCommand(Message message);
}