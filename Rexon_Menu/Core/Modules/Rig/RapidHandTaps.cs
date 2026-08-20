// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.RapidHandTaps
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Rapid Hand Taps", "Rig", "Removes hand tap cooldown.", false, 41, ModType.Toggle, false)]
internal class RapidHandTaps : MonoBehaviour
{

	private void Update()
	{
		if (GorillaTagger.Instance != null)
		{
			GorillaTagger.Instance.lastLeftTap = 0f;
			GorillaTagger.Instance.lastRightTap = 0f;
		}
	}
}
