// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Critter.HoneySpammer
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Critter;

[Mod("Honey Spammer [MASTER]", "Critter", "Spam honey at hand position.", false, 7, ModType.Toggle, false)]
internal sealed class HoneySpammer : MonoBehaviour
{
	private void Update()
	{
		if (ControllerInputPoller.instance.rightGrab)
		{
			CritterUtilities.SpawnHoney(
				GorillaTagger.Instance.rightHandTransform.position,
				Quaternion.identity,
				0.04f);
		}
	}
}
