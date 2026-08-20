// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Critter.FoodSpammer
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Recovered.Obfuscated;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Critter;

[Mod("Food Spammer [MASTER]", "Critter", "Spam food at hand position.", false, 8, ModType.Toggle, false)]
internal sealed class FoodSpammer : MonoBehaviour
{
	private void Update()
	{
		if (ControllerInputPoller.instance.rightGrab)
		{
			Transform hand = GorillaTagger.Instance.rightHandTransform;
			CritterUtilities.SpawnFoodNear(
				hand.position,
				Quaternion.LookRotation(hand.position, hand.position),
				Vector3.zero,
				CritterSizeSetting.CurrentScale,
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
