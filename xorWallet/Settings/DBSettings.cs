// ReSharper disable InconsistentNaming

namespace xorWallet.Settings;

public class DBSettings
{
    public required string AtlasURI { get; init; }
    public required string DatabaseName { get; init; }
}