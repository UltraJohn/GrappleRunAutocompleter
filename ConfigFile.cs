using IdleSlayerMods.Common.Config;
using MelonLoader;
using UnityEngine;

namespace GrappleRunAutocompleter;

internal sealed class ConfigFile(string configName) : BaseConfig(configName)
{
    internal MelonPreferences_Entry<bool> EnableSkip;
    internal MelonPreferences_Entry<KeyCode> ToggleKey;

    protected override void SetBindings()
    {
        ToggleKey = Bind("Grapple Run Autocompleter", "ToggleKey", KeyCode.G, "Keybind for enabling/disabling grapple run skip");
        EnableSkip = Bind("Grapple Run Autocompleter", "Skip Grapple Run", true,
            "Enable autocompletion of grapple run minigame.");
    }
}
