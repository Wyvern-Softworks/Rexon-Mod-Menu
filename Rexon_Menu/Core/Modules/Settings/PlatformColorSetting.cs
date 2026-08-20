// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.PlatformColorSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Platform Color: Purple", "Settings", "Cycle platform colors", false, 2, ModType.Toggle, false)]
internal class PlatformColorSetting : MonoBehaviour
{
	internal readonly struct ColorOption
	{
		public readonly string Name;
		public readonly Color Color;

		public ColorOption(string name, Color color)
		{
			Name = name;
			Color = color;
		}
	}

	internal static readonly ColorOption[] Options =
	{
		new ColorOption("Purple", new Color(0.32f, 0.02f, 0.72f, 1f)),
		new ColorOption("Red", new Color(0.8f, 0.1f, 0.1f, 1f)),
		new ColorOption("Blue", new Color(0.1f, 0.2f, 0.8f, 1f)),
		new ColorOption("Green", new Color(0.1f, 0.6f, 0.1f, 1f)),
		new ColorOption("Orange", new Color(0.9f, 0.4f, 0f, 1f)),
		new ColorOption("Cyan", new Color(0f, 0.7f, 0.8f, 1f)),
		new ColorOption("Pink", new Color(0.9f, 0.3f, 0.6f, 1f)),
		new ColorOption("Yellow", new Color(0.9f, 0.9f, 0.1f, 1f)),
		new ColorOption("White", Color.white),
		new ColorOption("Black", new Color(0.1f, 0.1f, 0.1f, 1f)),
		new ColorOption("Rainbow", Color.white)
	};

	private static int currentIndex;
	public static Color CurrentColor { get; internal set; } = Options[0].Color;
	public static bool IsRainbow { get; internal set; }

	public static int CurrentIndex
	{
		get => currentIndex;
		set => currentIndex = Mathf.Clamp(value, 0, Options.Length - 1);
	}

	private void OnEnable()
	{
		CurrentIndex = (CurrentIndex + 1) % Options.Length;
		ApplyCurrentColor();
	}

	private static void ApplyCurrentColor()
	{
		ColorOption option = Options[CurrentIndex];
		IsRainbow = option.Name == "Rainbow";
		if (!IsRainbow)
		{
			CurrentColor = option.Color;
		}
		BundleManager.SetPlatformColorStatusText("Platform Color: " + option.Name);
	}
}
