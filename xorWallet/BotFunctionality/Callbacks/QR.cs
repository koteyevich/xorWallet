using Microsoft.Extensions.Logging;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Telegram.Bot;
using Telegram.Bot.Types;
using xorWallet.BotFunctionality.Interfaces;
using xorWallet.Helpers;
using xorWallet.Settings;

namespace xorWallet.BotFunctionality.Callbacks;

public class QR : ICallback
{
    public string Name => "qr";

    private readonly ITelegramBotClient _bot;
    private readonly IBotInfo _info;
    private readonly ILogger<QR> _logger;

    public QR(ITelegramBotClient bot, IBotInfo info, ILogger<QR> logger)
    {
        _bot = bot;
        _info = info;
        _logger = logger;
    }

    public async Task ExecuteAsync(CallbackQuery callbackQuery)
    {
        var data = string.Join(
            '_',
            Parser.ParseArguments(callbackQuery.Data!, '_').Skip(1).ToArray()
        ); // qr_check_kotEy3v1ch... => // check_kotEy3v1ch...

        var url = new PayloadGenerator.Url($"https://t.me/{_info.Me.Username}?start={data}");
        var qrGenerator = new QRCodeGenerator();

        var qr = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.H);
        var pngQr = new PngByteQRCode(qr);

        var qrTransparent = pngQr.GetGraphic(20, [255, 255, 255, 255], [0, 0, 0, 0], false);

        await using var stream = new MemoryStream();

        using var qrImage = Image.Load(qrTransparent);
        using var qrBg = await Image.LoadAsync("qr_bg.jpg");

        qrImage.Mutate(i =>
        {
            i.Resize(new ResizeOptions
            {
                Size = new Size(qrBg.Width - qrBg.Width / 5, qrBg.Height - qrBg.Height / 5),
                Mode = ResizeMode.Pad,
                Sampler = KnownResamplers.Bicubic
            });
        });

        var location = new Point(
            (qrBg.Width - qrImage.Width) / 2,
            (qrBg.Height - qrImage.Height) / 2
        );

        qrBg.Mutate(i => i.DrawImage(qrImage, location, 1f));

        await qrBg.SaveAsync(stream, new JpegEncoder());

        stream.Position = 0;
        await _bot.SendPhoto(callbackQuery.Message.Chat.Id, new InputFileStream(stream));
    }
}