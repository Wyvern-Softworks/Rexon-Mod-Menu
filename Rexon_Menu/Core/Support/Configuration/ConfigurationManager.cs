// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ConfigurationManager
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Modules.Overpowered;
using Rexon_Menu.Core.Modules.World;
using Rexon_Menu.Core.Utilities;
using UnityEngine;

namespace Recovered.Obfuscated;

internal static class ConfigurationManager
{
	private const string SaveFileName = "RexonSave.txt";
	private const string SettingsSection = "[SETTINGS]";
	private const string ModsSection = "[MODS]";

	private static readonly HashSet<string> ExcludedModNames = new HashSet<string>();
	private static string savePath;
	private static bool autoLoadEnabled;
	private static bool hasLoaded;

	public static bool AutoLoadEnabled
	{
		get
		{
			EnsureLoaded();
			return autoLoadEnabled;
		}
		set => autoLoadEnabled = value;
	}

	internal static string GetSavePath()
	{
		if (savePath == null)
		{
			savePath = Path.Combine(Paths.ConfigPath, SaveFileName);
		}

		return savePath;
	}

	internal static void EnsureLoaded()
	{
		if (hasLoaded)
		{
			return;
		}

		hasLoaded = true;
		string path = GetSavePath();
		if (!File.Exists(path))
		{
			return;
		}

		string[] lines = File.ReadAllLines(path);
		if (lines.Length > 0 && bool.TryParse(lines[0], out bool enabled))
		{
			autoLoadEnabled = enabled;
		}
	}

	public static void ExcludeModFromAutoLoad(string modName)
	{
		ExcludedModNames.Add(modName);
	}

	internal static void Save()
	{
		using (StreamWriter writer = new StreamWriter(GetSavePath(), append: false))
		{
			writer.WriteLine(autoLoadEnabled);
			writer.WriteLine(SettingsSection);
			writer.WriteLine("AntiReportDistance=" + AntiReport.ReportDistance.ToString("F2"));
			writer.WriteLine("AntiReportTypeIndex=" + AntiReport.ResponseModeIndex);
			writer.WriteLine("ThemeIndex=" + ThemeSetting.CurrentIndex);
			writer.WriteLine("PlatformColorIndex=" + PlatformColorSetting.CurrentIndex);
			writer.WriteLine("GunTracerIndex=" + GunTracerSetting.CurrentIndex);
			writer.WriteLine("GunColorIndex=" + GunColorSetting.CurrentIndex);
			writer.WriteLine("GunSoundEnabled=" + GunSoundSetting.SoundEnabled);
			writer.WriteLine("VisualThemeIndex=" + VisualThemeSetting.CurrentIndex);
			writer.WriteLine("ProjectileColorIndex=" + GameNetworkUtilities.ProjectileColorIndex);
			writer.WriteLine("ProjectileSpeedIndex=" + GameNetworkUtilities.ProjectileSpeedIndex);
			writer.WriteLine("ImpactColorIndex=" + GameNetworkUtilities.ImpactColorIndex);
			writer.WriteLine("LagPower=" + Rexon_Menu.Core.Modules.Settings.LagPower.Power);
			writer.WriteLine("AudioIndex=" + AudioIndex.CurrentIndex);
			writer.WriteLine("SteamArmsLength=" + Rexon_Menu.Core.Modules.Rig.SteamLongArms.ScaleMultiplier.ToString("F2"));
			writer.WriteLine("SpeedBoostLevel=" + Rexon_Menu.Core.Modules.Movement.SpeedBoost.Multiplier.ToString("F2"));
			writer.WriteLine("GravityValue=" + Gravity.Multiplier.ToString("F2"));
			writer.WriteLine("SoundboardHearSelf=" + (SoundboardAudioManager.HearSelf ? "1" : "0"));
			writer.WriteLine("MenuHandIsLeft=" + (MenuHandSetting.IsLeftHand ? "1" : "0"));
			writer.WriteLine("MenuStyleIndex=" + MenuStyleSetting.CurrentStyle);
			writer.WriteLine(ModsSection);

			if (autoLoadEnabled && ModManager.Instance != null)
			{
				foreach (Component component in ModManager.Instance.gameObject.GetComponents<Component>())
				{
					if (component == null)
					{
						continue;
					}

					Mod mod = component.GetType().GetCustomAttribute<Mod>();
					if (mod != null && !mod.Name.Contains("Auto Load") && !ExcludedModNames.Contains(mod.Name))
					{
						writer.WriteLine(mod.Name);
					}
				}
			}
		}

		ExcludedModNames.Clear();
	}

	public static void SaveIfAutoLoadEnabled()
	{
		EnsureLoaded();
		if (autoLoadEnabled)
		{
			Save();
		}
	}
}
