// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GadgetGunBase
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using UnityEngine;

namespace Recovered.Obfuscated;

internal abstract class GadgetGunBase : MonoBehaviour
{
	protected abstract string ModId { get; }
	protected abstract string GadgetName { get; }

	private void Update()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.IsSuperInfectionMode() || !PhotonNetwork.IsMasterClient)
		{
			GunController.Release(ModId);
			return;
		}

		GameNetworkUtilities.CacheGameEntityTypeIds();
		if (!GameNetworkUtilities.EntityTypeIdsByName.TryGetValue(GadgetName, out int entityTypeId))
		{
			return;
		}

		GunController.GunResult gunResult = GunController.GetGunResult(ModId, targetPlayers: false, 0.01f);
		if (gunResult.IsActive && gunResult.IsShooting && gunResult.CanFire && gunResult.Hit.point != Vector3.zero)
		{
			GunController.MarkFired(ModId);
			GameNetworkUtilities.SpawnGameEntity(entityTypeId, gunResult.Hit.point, Random.rotation);
		}
	}

	private void OnDisable()
	{
		GunController.Release(ModId);
	}
}
