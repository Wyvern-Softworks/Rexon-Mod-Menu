// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.AntiGuardianModsSelf
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Anti Guardian Mods Self", "Safety", "Block guardian mod effects on self.", false, 8, ModType.Toggle, false)]
internal class AntiGuardianModsSelf : MonoBehaviour
{
	internal static bool IsEnabled;


	private void OnEnable()
	{
		IsEnabled = true;
	}

	private void OnDisable()
	{
		IsEnabled = false;
	}
}
