namespace xorWallet.Helpers;

public class Parser
{
    /// <summary>
    /// Parses the first line of the string, removes "/[command]" and then splits the arguments using delimiter.
    /// </summary>
    /// <param name="str">Original string</param>
    /// <param name="delimiter">Optional. Delimiter that will be used to return arguments</param>
    /// <returns>Arguments in an array</returns>
    public static string[] ParseCommandArguments(string str, char delimiter = ' ')
    {
        var parts = str.Split(' ', 2);
        if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
            return []; // no arguments

        var parameters = parts[1];
        return parameters.Split(delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Parses the first line of the string, splits the arguments using delimiter.
    /// </summary>
    /// <param name="str">Original string</param>
    /// <param name="delimiter">Optional. Delimiter that will be used to return arguments</param>
    /// <returns>Arguments in an array</returns>
    public static string[] ParseArguments(string str, char delimiter = ' ')
    {
        return str.Split(delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }


    /// <summary>
    /// Parses the second and below lines (if they exist) of the original string.
    /// Example of usage: Telegram Moderation Bot that allows to parse a reason for the punishment.
    /// </summary>
    /// <param name="str">Original string</param>
    /// <returns>If the second and below lines exist - contents of those lines. Else - null</returns>
    public static string? ParseLinesBelow(string str)
    {
        return str.Contains('\n')
            ? str[(str.IndexOf('\n') + 1)..].Trim()
            : null;
    }
}