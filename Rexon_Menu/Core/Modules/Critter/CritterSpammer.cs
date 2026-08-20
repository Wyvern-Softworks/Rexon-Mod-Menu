// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Critter.CritterSpammer
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Recovered.Obfuscated;
using UnityEngine;

using CreatureState = CrittersPawn.CreatureState;

namespace Rexon_Menu.Core.Modules.Critter;

[Mod("Critter Spammer [MASTER]", "Critter", "Spam critters at hand position.", false, 1, ModType.Toggle, false)]
internal sealed class CritterSpammer : MonoBehaviour
{
	private void Update()
	{
		if (ControllerInputPoller.instance.rightGrab)
		{
			CritterUtilities.SpawnCritter(
				GorillaTagger.Instance.rightHandTransform.position,
				Vector3.zero,
				CritterSizeSetting.CurrentScale,
				(CreatureState)3,
				0.04f);
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
