using System;
using System.Linq;

namespace UNBEATAP.AP;

public static class APVersion
{
    private static string ModVersion => PluginReleaseInfo.PLUGIN_VERSION;
    private static Version[] CompatibleVersions => PluginReleaseInfo.COMPATIBLE_VERSIONS;


    public static bool CheckConnectionCompatible(string apworldVersion, string[] apworldCompatibleVersions)
    {
        Version modVersion = new Version(ModVersion);
        Version apVersion = new Version(apworldVersion);

        if(modVersion == apVersion)
        {
            return true;
        }

        if(CompatibleVersions.Contains(apVersion))
        {
            return true;
        }

        if(apworldCompatibleVersions.Any(x => new Version(x) == modVersion))
        {
            return true;
        }

        return false;
    }
}