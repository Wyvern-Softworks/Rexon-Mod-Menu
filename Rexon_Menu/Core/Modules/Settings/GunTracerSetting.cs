// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GunTracerSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Gun Tracer: Straight", "Settings", "Cycle tracer styles", false, 3, ModType.Toggle, false)]
internal class GunTracerSetting : MonoBehaviour
{
	internal readonly struct TracerOption
	{
		public readonly string Name;
		public readonly GunController.TracerStyle Style;

		public TracerOption(string name, GunController.TracerStyle style)
		{
			Name = name;
			Style = style;
		}
	}

	internal static readonly TracerOption[] Options =
	{
		new TracerOption("Straight", GunController.TracerStyle.Straight),
		new TracerOption("Wavy", GunController.TracerStyle.Wavy),
		new TracerOption("Wiggle", GunController.TracerStyle.Wiggle),
		new TracerOption("Tether", GunController.TracerStyle.Tether),
		new TracerOption("Zigzag", GunController.TracerStyle.Zigzag),
		new TracerOption("Off", GunController.TracerStyle.Off)
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
		ApplyCurrentTracer();
	}

	private static void ApplyCurrentTracer()
	{
		TracerOption option = Options[CurrentIndex];
		GunController.CurrentTracer = option.Style;
		BundleManager.SetGunTracerStatusText("Gun Tracer: " + option.Name);
	}
}
