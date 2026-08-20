// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.RemoveSnowballKnockback
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Remove Snowball Knockback", "Safety", "Removes snowball knockback.", false, 9, ModType.Toggle, false)]
internal class RemoveSnowballKnockback : MonoBehaviour
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
