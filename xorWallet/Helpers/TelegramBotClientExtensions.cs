using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

// ReSharper disable InvalidXmlDocComment

namespace xorWallet.Helpers;

public static class TelegramBotClientExtensions
{
    /// <summary>Method to send text messages in the same messageThreadId and chat id (chat id can still me explicitly specified)
    /// (every other argument except for which are specified in this doc are the same as Telegram.Bot SendMessage method)</summary>
    /// <param name="message">Message instance.</param>
    public static async Task<Message> SendMessage(this ITelegramBotClient botClient,
        Message message,
        string text,
        ChatId? chatId = null,
        ParseMode parseMode = default,
        ReplyParameters? replyParameters = null,
        ReplyMarkup? replyMarkup = null,
        LinkPreviewOptions? linkPreviewOptions = null,
        IEnumerable<MessageEntity>? entities = null,
        int? messageThreadId = null,
        bool disableNotification = false,
        bool protectContent = false,
        string? messageEffectId = null,
        string? businessConnectionId = null,
        bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        return await botClient.SendRequest(new SendMessageRequest
        {
            Text = text,
            ParseMode = parseMode,
            ChatId = chatId ?? message.Chat.Id,
            ReplyParameters = replyParameters,
            ReplyMarkup = replyMarkup,
            LinkPreviewOptions = linkPreviewOptions,
            MessageThreadId = messageThreadId ?? message.MessageThreadId,
            Entities = entities,
            DisableNotification = disableNotification,
            ProtectContent = protectContent,
            MessageEffectId = messageEffectId,
            BusinessConnectionId = businessConnectionId,
            AllowPaidBroadcast = allowPaidBroadcast,
        }, cancellationToken);
    }

    /// <summary>Use this method to send animation files (GIF or H.264/MPEG-4 AVC video without sound).
    /// (every other argument except for which are specified in this doc are the same as Telegram.Bot SendAnimation method)</summary>
    /// <remarks>Bots can currently send animation files of up to 50 MB in size, this limit may be changed in the future.</remarks>
    /// <param name="message">Original Message</param>
    public static async Task<Message> SendAnimation(
        this ITelegramBotClient botClient,
        Message message,
        InputFile animation,
        ChatId? chatId = null,
        string? caption = null,
        ParseMode parseMode = default,
        ReplyParameters? replyParameters = null,
        ReplyMarkup? replyMarkup = null,
        int? duration = null,
        int? width = null,
        int? height = null,
        InputFile? thumbnail = null,
        int? messageThreadId = null,
        IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false,
        bool hasSpoiler = false,
        bool disableNotification = false,
        bool protectContent = false,
        string? messageEffectId = null,
        string? businessConnectionId = null,
        bool allowPaidBroadcast = false,
        int? directMessagesTopicId = null,
        SuggestedPostParameters? suggestedPostParameters = null,
        CancellationToken cancellationToken = default
    )
    {
        return await botClient.SendRequest(new SendAnimationRequest
        {
            ChatId = chatId ?? message.Chat.Id,
            Animation = animation,
            Caption = caption,
            ParseMode = parseMode,
            ReplyParameters = replyParameters,
            ReplyMarkup = replyMarkup,
            Duration = duration,
            Width = width,
            Height = height,
            Thumbnail = thumbnail,
            MessageThreadId = messageThreadId ?? message.MessageThreadId,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            HasSpoiler = hasSpoiler,
            DisableNotification = disableNotification,
            ProtectContent = protectContent,
            MessageEffectId = messageEffectId,
            BusinessConnectionId = businessConnectionId,
            AllowPaidBroadcast = allowPaidBroadcast,
            DirectMessagesTopicId = directMessagesTopicId,
            SuggestedPostParameters = suggestedPostParameters
        }, cancellationToken).ConfigureAwait(false);
    }
}