// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.FoodGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Food Gun [MASTER]", "Critter", "Spawn food at aim point.", false, 6, ModType.Toggle, false)]
internal class FoodGun : MonoBehaviour
{


	private void Update()
	{
		GunController.GunResult gunResult = GunController.GetGunResult("FoodGun", targetPlayers: false);
		if (gunResult.IsShooting && gunResult.IsActive)
		{
			CritterUtilities.SpawnFoodNear(
				gunResult.Hit.point, Quaternion.identity, Vector3.zero, CritterSizeSetting.CurrentScale, 0.04f);
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		GunController.Release("FoodGun");
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
