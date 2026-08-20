// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.HoneyGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Honey Gun [MASTER]", "Critter", "Spawn honey at aim point.", false, 5, ModType.Toggle, false)]
internal class HoneyGun : MonoBehaviour
{

	private void Update()
	{
		GunController.GunResult gunResult = GunController.GetGunResult("HoneyGun", targetPlayers: false);
		if (gunResult.IsShooting && gunResult.IsActive)
		{
			CritterUtilities.SpawnHoney(gunResult.Hit.point, Quaternion.identity, 0.04f);
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		GunController.Release("HoneyGun");
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
