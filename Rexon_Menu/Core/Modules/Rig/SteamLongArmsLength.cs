// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.SteamLongArmsLength
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Steam Long Arms Length", "Rig", "Triggers (VR) / Click (PC) to adjust.", false, 47, ModType.Toggle, false)]
internal sealed class SteamLongArmsLength : MonoBehaviour
{
	private const string LabelFormat = "Steam Long Arms Length: {0:F2}";
	private static readonly float[] DesktopPresets = { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f };

	private float _lastAdjustmentAt;
	private float _displayedMultiplier;

	private void OnEnable()
	{
		if (!XRSettings.isDeviceActive)
		{
			int nextPreset = 0;
			for (int index = 0; index < DesktopPresets.Length; index++)
			{
				if (Mathf.Abs(SteamLongArms.ScaleMultiplier - DesktopPresets[index]) < 0.01f)
				{
					nextPreset = (index + 1) % DesktopPresets.Length;
					break;
				}
			}

			SteamLongArms.ScaleMultiplier = DesktopPresets[nextPreset];
			UpdateLabel();
			ConfigurationManager.SaveIfAutoLoadEnabled();
			Destroy(this);
			return;
		}

		_displayedMultiplier = SteamLongArms.ScaleMultiplier;
		UpdateLabel();
	}

	private void Update()
	{
		if (Time.time <= _lastAdjustmentAt + 0.02f)
		{
			return;
		}

		float rightTrigger = ControllerInputPoller.instance.rightControllerIndexFloat;
		float leftTrigger = ControllerInputPoller.instance.leftControllerIndexFloat;
		if (rightTrigger > 0.5f)
		{
			_lastAdjustmentAt = Time.time;
			SteamLongArms.ScaleMultiplier = Mathf.Clamp(SteamLongArms.ScaleMultiplier + 0.02f, 0.5f, 3f);
		}
		else if (leftTrigger > 0.5f)
		{
			_lastAdjustmentAt = Time.time;
			SteamLongArms.ScaleMultiplier = Mathf.Clamp(SteamLongArms.ScaleMultiplier - 0.02f, 0.5f, 3f);
		}

		if (Mathf.Abs(_displayedMultiplier - SteamLongArms.ScaleMultiplier) > 0.01f)
		{
			_displayedMultiplier = SteamLongArms.ScaleMultiplier;
			UpdateLabel();
		}
	}

	private static void UpdateLabel()
	{
		BundleManager.SetSteamArmsLengthStatusText(string.Format(LabelFormat, SteamLongArms.ScaleMultiplier));
	}

	private void OnDisable()
	{
		UpdateLabel();
		ConfigurationManager.SaveIfAutoLoadEnabled();
	}
}
