using HarmonyLib;
using IdleSlayerMods.Common.Extensions;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks.Dataflow;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using static MelonLoader.MelonLogger;
using Input = UnityEngine.Input;

namespace GrappleRunAutocompleter;

public class GrappleRunAutocompleter : MonoBehaviour
{
	private bool modEnabled;
	private void Awake()
	{
		modEnabled = Plugin.Config.EnableSkip.Value;
	}


	private void LateUpdate()
	{

		if (Input.GetKeyDown(Plugin.Config.ToggleKey.Value))
		{
			ToggleMod();
		}

#if DEBUG
		if (Input.GetKeyDown(KeyCode.P))
		{
			foreach(BaseMap m in MapController.instance.maps)
			{
				if (m.name == "popup_grapple_run_title")
				{
					MapController.instance.ChangeMap(m);
					break;
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			Plugin.Logger.Msg("ODMGearMap.startingGravity: " + ODMGearMapController.instance.startingGravity);
			Plugin.Logger.Msg("ODMGearMap.currentState:" + ODMGearMapController.instance.currentState);
			Plugin.Logger.Msg("ODMGearMap.currentODMMap:" + ODMGearMapController.instance.currentODMMap.name);
			if(ODMGearMapController.instance.bonusStartSlider != null && ODMGearMapController.instance.bonusStartSlider.sliderReady)
			{
				ODMGearMapController.instance.StartBonus();
			}
		}
#endif
	}

	private void ToggleMod()
	{
		Plugin.Config.EnableSkip.Value = !Plugin.Config.EnableSkip.Value;

		if (Plugin.Config.EnableSkip.Value)
		{
			Plugin.Logger.Msg("Enabled Grapple run skip.");
			Plugin.ModHelperInstance.ShowNotification("Grapple Run Skip Enabled!", true);
		}
		else
		{
			Plugin.Logger.Msg("Disabled Grapple run skip.");
			Plugin.ModHelperInstance.ShowNotification("Grapple Run Skip Disabled!", false);
		}
}

	// BonusStartSlider.SetRandomPuzzle runs twice for some reason. cache it for one second and release so it works again next time the minigame loads.
	private static bool once = false;

	[HarmonyPatch(typeof(BonusStartSlider), "SetRandomPuzzle")]
	public class SkipGrappleRunPatch1
	{
		private static void Postfix(BonusStartSlider __instance)
		{
			if (!Plugin.Config.EnableSkip.Value)
			{
				return;
			}
			if (MapController.instance.selectedMap.name != "popup_grapple_run_title")
			{
				return;
			}
			if (__instance.sliderReady && !once)
			{
				once = true;
				MelonCoroutines.Start(ExecuteAfterDelay(1.0f));
				ODMGearMapController.instance.bonusStartSlider.confirmAction.Invoke();
			}
		}
	}

	private static IEnumerator ExecuteAfterDelay(float delayInSeconds)
	{
		yield return new WaitForSeconds(delayInSeconds);
		once = false;
	}

	[HarmonyPatch(typeof(ODMGearMapController), "StartBonus")]
	public class SkipGrappleRunPatch2
	{
		private static void Postfix(ODMGearMapController __instance)
		{
			if (!Plugin.Config.EnableSkip.Value)
			{
				return;
			}
			//Plugin.Logger.Msg("Grapple Run StartBonus(). Map name: " + MapController.instance.selectedMap.name);
			if (MapController.instance.selectedMap.name != "popup_grapple_run_title")
			{
				return;
			}
			//Plugin.Logger.Msg("this is test phase");
			__instance.currentODMMap.finishAtDistance = 0;
			MelonCoroutines.Start(BeginMoving());
		}

		private static IEnumerator BeginMoving()
		{
			yield return new WaitForSeconds(4f);
			var click_spot = new PointerEventData(EventSystem.current) { position = new Vector2(Screen.width / 2f, Screen.height / 2f) };
			ODMGasRelease gas = GameObject.Find("Gas Button").GetComponent<ODMGasRelease>();
			//Plugin.Logger.Msg("gas exists? " + gas.name);
			gas.OnPointerDown(click_spot);
			yield return new WaitForSeconds(1f);
			gas.OnPointerUp(click_spot);
		}
	}
}
