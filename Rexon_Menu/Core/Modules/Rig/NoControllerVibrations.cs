// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.NoControllerVibrations
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Controller = OVRInput.Controller;

namespace Recovered.Obfuscated;

[Mod("No Controller Vibrations", "Rig", "Disables controller vibrations.", false, 51, ModType.Toggle, false)]
internal class NoControllerVibrations : MonoBehaviour
{
	private float _originalHapticStrength;

	private float _originalHapticDuration;

	private bool _settingsCaptured;


	private void OnEnable()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		if (instance != null)
		{
			_originalHapticStrength = instance.tapHapticStrength;
			_originalHapticDuration = instance.tapHapticDuration;
			_settingsCaptured = true;
			instance.tapHapticStrength = 0f;
			instance.tapHapticDuration = 0f;
		}
	}

	private void Update()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		if (instance != null)
		{
			instance.tapHapticStrength = 0f;
			instance.tapHapticDuration = 0f;
		}
		OVRInput.SetControllerVibration(0f, 0f, (Controller)1);
		OVRInput.SetControllerVibration(0f, 0f, (Controller)2);
	}

	private void OnDisable()
	{
		RestoreControllerVibrations();
	}

	private void RestoreControllerVibrations()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		if (instance != null && _settingsCaptured)
		{
			instance.tapHapticStrength = _originalHapticStrength;
			instance.tapHapticDuration = _originalHapticDuration;
		}
	}
}

