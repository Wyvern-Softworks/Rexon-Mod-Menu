// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.FoodMinigun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Food Minigun [MASTER]", "Critter", "Fire food toward aim point.", false, 9, ModType.Toggle, false)]
internal class FoodMinigun : MonoBehaviour
{


	private void Update()
	{
		GunController.GunResult gunResult = GunController.GetGunResult("FoodMinigun", targetPlayers: false);
		if (gunResult.IsShooting && gunResult.IsActive)
		{
			float projectileSpeed = GameNetworkUtilities.ProjectileSpeeds[GameNetworkUtilities.ProjectileSpeedIndex];
			Vector3 direction = gunResult.Hit.point - GorillaTagger.Instance.rightHandTransform.position;
			Vector3 velocity = projectileSpeed * direction.normalized;
			CritterUtilities.SpawnFoodNear(
				GorillaTagger.Instance.rightHandTransform.position, Quaternion.LookRotation(direction), velocity,
				CritterSizeSetting.CurrentScale, 0.04f);
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		GunController.Release("FoodMinigun");
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
