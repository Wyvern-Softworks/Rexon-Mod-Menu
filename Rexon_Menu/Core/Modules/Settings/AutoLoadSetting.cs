// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.AutoLoadSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Auto Load: Off", "Settings", "Save and load mods on startup", false, 11, ModType.Toggle, false)]
internal class AutoLoadSetting : MonoBehaviour
{
	private void OnEnable()
	{
		ConfigurationManager.EnsureLoaded();
		ConfigurationManager.AutoLoadEnabled = !ConfigurationManager.AutoLoadEnabled;
		ConfigurationManager.Save();
		ConfigurationManager.EnsureLoaded();
		string status = ConfigurationManager.AutoLoadEnabled ? "On" : "Off";
		BundleManager.SetAutoLoadStatusText("Auto Load: " + status);
	}
}
