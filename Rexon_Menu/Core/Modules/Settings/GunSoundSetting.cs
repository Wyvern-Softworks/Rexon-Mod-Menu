// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GunSoundSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Gun Sound: Off", "Settings", "Toggle gun sound", false, 5, ModType.Toggle, false)]
internal class GunSoundSetting : MonoBehaviour
{
	public static bool SoundEnabled { get; set; }

	private void OnEnable()
	{
		SoundEnabled = !SoundEnabled;
		GunController.SoundEnabled = SoundEnabled;
		BundleManager.SetGunSoundStatusText("Gun Sound: " + (SoundEnabled ? "On" : "Off"));
	}
}
