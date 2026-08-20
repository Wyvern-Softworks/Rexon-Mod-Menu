// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CritterGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

using CreatureState = CrittersPawn.CreatureState;

namespace Recovered.Obfuscated;

[Mod("Critter Gun [MASTER]", "Critter", "Spawn critters at aim point.", false, 0, ModType.Toggle, false)]
internal class CritterGun : MonoBehaviour
{


	private void Update()
	{
		GunController.GunResult gunResult = GunController.GetGunResult("CritterGun", targetPlayers: false);
		if (gunResult.IsShooting && gunResult.IsActive)
		{
			CritterUtilities.SpawnCritter(
				gunResult.Hit.point, Vector3.zero, CritterSizeSetting.CurrentScale, (CreatureState)3, 0.04f);
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		GunController.Release("CritterGun");
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
