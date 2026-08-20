// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GunColorSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Gun Color: Purple", "Settings", "Cycle gun colors", false, 4, ModType.Toggle, false)]
internal class GunColorSetting : MonoBehaviour
{
	internal readonly struct ColorOption
	{
		public readonly string Name;
		public readonly Color IdleColor;
		public readonly Color ShootingColor;

		public ColorOption(string name, Color idleColor, Color shootingColor)
		{
			Name = name;
			IdleColor = idleColor;
			ShootingColor = shootingColor;
		}
	}

	internal static readonly ColorOption[] Options =
	{
		new ColorOption("Purple", new Color(0.35f, 0.01f, 0.41f), new Color(0.7f, 0.01f, 0.82f)),
		new ColorOption("Red", new Color(0.4f, 0.05f, 0.05f), new Color(0.9f, 0.1f, 0.1f)),
		new ColorOption("Blue", new Color(0.05f, 0.1f, 0.4f), new Color(0.1f, 0.3f, 0.9f)),
		new ColorOption("Green", new Color(0.05f, 0.3f, 0.05f), new Color(0.1f, 0.8f, 0.1f)),
		new ColorOption("Orange", new Color(0.4f, 0.2f, 0f), new Color(0.9f, 0.5f, 0f)),
		new ColorOption("Cyan", new Color(0f, 0.3f, 0.35f), new Color(0f, 0.8f, 0.9f)),
		new ColorOption("Pink", new Color(0.4f, 0.1f, 0.25f), new Color(0.9f, 0.3f, 0.6f)),
		new ColorOption("Yellow", new Color(0.4f, 0.4f, 0.05f), new Color(0.9f, 0.9f, 0.1f)),
		new ColorOption("White", new Color(0.4f, 0.4f, 0.4f), Color.white),
		new ColorOption("Rainbow", Color.white, Color.white)
	};

	private static int currentIndex;

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
		GunController.IsRainbow = option.Name == "Rainbow";
		if (!GunController.IsRainbow)
		{
			GunController.ColorIdle = option.IdleColor;
			GunController.ColorShooting = option.ShootingColor;
		}
		BundleManager.SetGunColorStatusText("Gun Color: " + option.Name);
	}
}
