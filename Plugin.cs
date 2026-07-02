using IdleSlayerMods.Common;
using MelonLoader;
using HarmonyLib;
using MyPluginInfo = GrappleRunAutocompleter.MyPluginInfo;
using Plugin = GrappleRunAutocompleter.Plugin;

[assembly: MelonInfo(typeof(Plugin), MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, MyPluginInfo.PLUGIN_AUTHOR)]
[assembly: MelonAdditionalDependencies("IdleSlayerMods.Common")]

namespace GrappleRunAutocompleter;

public class Plugin : MelonMod
{
    internal static ConfigFile Config;
    internal static readonly MelonLogger.Instance Logger = Melon<Plugin>.Logger;
    
    public override void OnInitializeMelon()
    {
        Config = new(MyPluginInfo.PLUGIN_GUID);
        Logger.Msg($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		//new HarmonyLib.Harmony("Skip Grapple Run").PatchAll();
	}

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName != "Game") return;
        ModUtils.RegisterComponent<GrappleRunAutocompleter>();
    }
}
