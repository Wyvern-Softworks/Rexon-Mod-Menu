// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ThemeSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Theme: Original", "Settings", "Click to cycle themes", false, 1, ModType.Toggle, false)]
internal class ThemeSetting : MonoBehaviour
{
	internal readonly struct ThemeData
	{
		public readonly string Name;
		public readonly Color Color;

		public ThemeData(string name, Color color)
		{
			Name = name;
			Color = color;
		}
	}

	internal static readonly ThemeData[] Themes =
	{
		new ThemeData("Original", new Color(0.196f, 0.051f, 0.357f, 1f)),
		new ThemeData("Dark", new Color(0.1f, 0.1f, 0.1f, 1f)),
		new ThemeData("Red", new Color(0.5f, 0.08f, 0.08f, 1f)),
		new ThemeData("Blue", new Color(0.08f, 0.15f, 0.5f, 1f)),
		new ThemeData("Green", new Color(0.08f, 0.35f, 0.08f, 1f)),
		new ThemeData("Purple", new Color(0.35f, 0.08f, 0.5f, 1f)),
		new ThemeData("Orange", new Color(0.5f, 0.25f, 0f, 1f)),
		new ThemeData("Cyan", new Color(0f, 0.35f, 0.45f, 1f)),
		new ThemeData("Pink", new Color(0.5f, 0.15f, 0.35f, 1f)),
		new ThemeData("Gold", new Color(0.5f, 0.4f, 0f, 1f)),
		new ThemeData("White", new Color(0.7f, 0.7f, 0.7f, 1f)),
		new ThemeData("Rainbow", Color.white)
	};

	private static int currentIndex;

	public static int CurrentIndex
	{
		get => currentIndex;
		set => currentIndex = Mathf.Clamp(value, 0, Themes.Length - 1);
	}

	private void OnEnable()
	{
		CurrentIndex = (CurrentIndex + 1) % Themes.Length;
		ApplyCurrentTheme();
	}

	private static void ApplyCurrentTheme()
	{
		ThemeData theme = Themes[CurrentIndex];
		BundleManager.RainbowThemeEnabled = theme.Name == "Rainbow";
		if (!BundleManager.RainbowThemeEnabled)
		{
			BundleManager.SetThemeColor(theme.Color);
		}
		BundleManager.SetThemeStatusText("Theme: " + theme.Name);
	}

	public static string GetCurrentThemeName()
	{
		return Themes[CurrentIndex].Name;
	}
}
