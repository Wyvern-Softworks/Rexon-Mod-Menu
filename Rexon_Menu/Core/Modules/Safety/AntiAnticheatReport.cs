// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Safety.AntiAnticheatReport
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Safety;

[Mod("Anti Anticheat Report", "Safety", "Blocks anticheat from sending reports about you.", false, 14, ModType.Toggle, false)]
internal class AntiAnticheatReport : MonoBehaviour
{
	public static bool IsEnabled;

	private void OnEnable()
	{
		IsEnabled = true;
	}

	private void OnDisable()
	{
		IsEnabled = false;
	}
}
