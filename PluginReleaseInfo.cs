using System;

namespace UNBEATAP;

public class PluginReleaseInfo
{
    public const string PLUGIN_GUID = "unbeatAP";
    public const string PLUGIN_NAME = "unbeatAP";
    public const string PLUGIN_VERSION = "0.6.1";

    public static readonly Version[] COMPATIBLE_VERSIONS =
    [
        new Version(0, 4, 0),
        new Version(0, 5, 0),
        new Version(0, 6, 0)
    ];
}