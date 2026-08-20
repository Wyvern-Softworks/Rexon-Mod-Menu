// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.MenuStyleSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Menu Style: Current", "Settings", "Cycle menu appearance.", false, 21, ModType.Toggle, false)]
internal class MenuStyleSetting : MonoBehaviour
{
	public static readonly string[] Styles = { "Current", "Pre-Revamp", "Legacy" };
	private static int currentStyle;

	public static int CurrentStyle
	{
		get => currentStyle;
		set => currentStyle = Mathf.Clamp(value, 0, Styles.Length - 1);
	}

	private void OnEnable()
	{
		CurrentStyle = (CurrentStyle + 1) % Styles.Length;
		BundleManager.SetMenuStyleStatusText("Menu Style: " + Styles[CurrentStyle]);
		LegacyMenu.SetMenuStyle(CurrentStyle);
		Object.Destroy(this);
	}
}
