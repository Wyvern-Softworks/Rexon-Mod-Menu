// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ResetSteamLongArms
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Reset Steam Long Arms", "Rig", "", false, 48, ModType.Toggle, false)]
internal class ResetSteamLongArms : MonoBehaviour
{

	private void OnEnable()
	{
		Rexon_Menu.Core.Modules.Rig.SteamLongArms.ScaleMultiplier = 1f;
		Object.Destroy(this);
	}
}
