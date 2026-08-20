// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CritterMinigun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

using CreatureState = CrittersPawn.CreatureState;

namespace Recovered.Obfuscated;

[Mod("Critter Minigun [MASTER]", "Critter", "Fire critters toward aim point.", false, 2, ModType.Toggle, false)]
internal class CritterMinigun : MonoBehaviour
{


	private void Update()
	{
		GunController.GunResult gunResult = GunController.GetGunResult("CritterMinigun", targetPlayers: false);
		if (gunResult.IsShooting && gunResult.IsActive)
		{
			float projectileSpeed = GameNetworkUtilities.ProjectileSpeeds[GameNetworkUtilities.ProjectileSpeedIndex];
			Vector3 direction = gunResult.Hit.point - GorillaTagger.Instance.rightHandTransform.position;
			Vector3 velocity = projectileSpeed * direction.normalized;
			CritterUtilities.SpawnCritter(
				GorillaTagger.Instance.rightHandTransform.position, velocity,
				CritterSizeSetting.CurrentScale, (CreatureState)3, 0.04f);
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		GunController.Release("CritterMinigun");
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
