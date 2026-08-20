// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.VisualThemeSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Visual Theme: Original", "Settings", "Theme for visual mods", false, 9, ModType.Toggle, false)]
internal class VisualThemeSetting : MonoBehaviour
{
	public static readonly string[] ThemeNames =
	{
		"Original", "Purple", "Blue", "Red", "Green", "Yellow",
		"Cyan", "Pink", "Orange", "White", "Rainbow"
	};

	private static readonly Color[] ThemeColors =
	{
		Color.white,
		new Color(0.6f, 0f, 1f, 0.9f),
		new Color(0f, 0.5f, 1f, 0.9f),
		new Color(1f, 0.1f, 0.1f, 0.9f),
		new Color(0.1f, 1f, 0.1f, 0.9f),
		new Color(1f, 1f, 0f, 0.9f),
		new Color(0f, 1f, 1f, 0.9f),
		new Color(1f, 0.4f, 0.7f, 0.9f),
		new Color(1f, 0.5f, 0f, 0.9f),
		new Color(1f, 1f, 1f, 0.9f),
		Color.white
	};

	private static int currentIndex;

	public static int CurrentIndex
	{
		get => currentIndex;
		set => currentIndex = Mathf.Clamp(value, 0, ThemeNames.Length - 1);
	}

	public static string CurrentTheme => ThemeNames[CurrentIndex];

	private void OnEnable()
	{
		CurrentIndex = (CurrentIndex + 1) % ThemeNames.Length;
		BundleManager.RefreshMenu();
		BundleManager.SetVisualThemeStatusText("Visual Theme: " + CurrentTheme);
	}

	public static Color GetCurrentColor()
	{
		if (CurrentIndex == ThemeNames.Length - 1)
		{
			return Color.HSVToRGB(Time.time * 0.2f % 1f, 0.8f, 1f);
		}
		return ThemeColors[CurrentIndex];
	}
}
