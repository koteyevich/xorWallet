using xorWallet.Settings;

namespace xorWallet.Helpers;

public class StartUrlGenerator
{
    private readonly IBotInfo _botInfo;

    public StartUrlGenerator(IBotInfo botInfo)
    {
        _botInfo = botInfo;
    }

    /// <summary>
    /// Generates a URL that will lead to private messages with the bot and automatically run the start command with the payload (example: /start qwerty123)
    /// </summary>
    /// <param name="data">The payload.</param>
    /// <returns>A URL.</returns>
    public string Generate(string data)
    {
        return $"https://t.me/{_botInfo.Me.Username}?start={data}";
    }
}