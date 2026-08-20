// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.DrawGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Draw Gun [CRITTER] [MASTER]", "Critter", "Draw with honey at aim point.", false, 15, ModType.Toggle, false)]
internal class DrawGun : MonoBehaviour
{

	private void Update()
	{
		GunController.GunResult gunResult = GunController.GetGunResult("DrawGunCritter", targetPlayers: false);
		if (gunResult.IsShooting && gunResult.IsActive)
		{
			Vector3 hitPoint = gunResult.Hit.point;
			Vector3 playerPosition = GTPlayer.Instance.transform.position;
			Quaternion rotation = Quaternion.LookRotation(hitPoint - playerPosition) * Quaternion.Euler(90f, 0f, 0f);
			CritterUtilities.SpawnHoney(hitPoint, rotation, 0.04f);
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		GunController.Release("DrawGunCritter");
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
