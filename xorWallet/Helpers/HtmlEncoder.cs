using System.Net;

namespace xorWallet.Helpers;

/// <summary>
/// This class can be used to clean a string that might break when formating text with HTML parser
/// </summary>
public class HtmlEncoder
{
    /// <summary>
    /// Converts a string into an HTML encoded string to be compatible with HTML parser.
    /// This should be running for strings that you are not in control of, for example, users' first name.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>A string that is compatible with HTML parser.</returns>
    public static string Encode(string input)
    {
        return WebUtility.HtmlEncode(input);
    }
}