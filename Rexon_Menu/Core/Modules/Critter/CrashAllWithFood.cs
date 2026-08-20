// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Critter.CrashAllWithFood
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Recovered.Obfuscated;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Critter;

[Mod("Crash All [CRITTER] [MASTER]", "Critter", "Crash all via critter food overflow.", false, 4, ModType.Toggle, false)]
internal sealed class CrashAllWithFood : MonoBehaviour
{
	private void Update()
	{
		if (ControllerInputPoller.instance.rightGrab)
		{
			CritterUtilities.CrashAllWithFood();
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
