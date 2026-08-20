// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ShowReports
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Show Reports", "Safety", "Shows report notifications when reports are sent.", false, 12, ModType.Toggle, false)]
internal class ShowReports : MonoBehaviour
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
