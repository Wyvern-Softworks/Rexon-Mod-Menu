// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CritterCrashGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Crash Gun [CRITTER] [MASTER]", "Critter", "Crash targeted player via critter.", false, 12, ModType.Toggle, false)]
internal class CritterCrashGun : MonoBehaviour
{


	private void Update()
	{
		GunController.GunResult gunResult = GunController.GetGunResult("CrashGunCritter");
		if (gunResult.IsShooting && gunResult.IsActive && gunResult.Target != null)
		{
			CritterUtilities.CrashPlayerWithFood(gunResult.Target);
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		GunController.Release("CrashGunCritter");
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
