// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.MenuHandSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Menu Hand: Right", "Settings", "Toggle menu between left and right hand.", false, 16, ModType.Toggle, false)]
internal class MenuHandSetting : MonoBehaviour
{
	public static bool IsLeftHand { get; set; }

	private void OnEnable()
	{
		IsLeftHand = !IsLeftHand;
		BundleManager.SetMenuHandStatusText("Menu Hand: " + (IsLeftHand ? "Left" : "Right"));
		Main.CloseVrMenu();
		ConfigurationManager.SaveIfAutoLoadEnabled();
		Object.Destroy(this);
	}
}
