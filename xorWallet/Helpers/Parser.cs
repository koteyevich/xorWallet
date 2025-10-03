namespace xorWallet.Helpers;

public class Parser
{
    /// <summary>
    /// Parses the first line of the string, splits using spaces and skips the first word (because that is /[command])
    /// </summary>
    /// <param name="str">Original string</param>
    /// <param name="delimiter">Optional. Delimiter that will be used to return arguments</param>
    /// <returns>Arguments in an array</returns>
    public static string[] ParseArguments(string str, char delimiter = ' ')
    {
        // /[command] arg1[delimiter]arg2[delimiter]arg3
        // gets turned into "arg1[delimiter]arg2[delimiter]arg3"
        // basically, we split by space to remove a "/[command]"
        var parameters = str.Split(' ').Skip(1).ToArray()[0];

        return parameters.Split('\n')[0].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
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