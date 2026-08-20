// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.RandomGadgetsGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Random Gadgets Gun", "Super Infection/Casual [MASTERCLIENT]", "Spawns random gadgets.", false, 21, ModType.Toggle, false)]
internal class RandomGadgetsGun : MonoBehaviour
{
	private const string GunId = "RandomGadgetsGun";

	private int _entityNameIndex;


	private void Update()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.IsSuperInfectionMode() || !PhotonNetwork.IsMasterClient)
		{
			GunController.Release(GunId);
			return;
		}
		GameNetworkUtilities.CacheGameEntityTypeIds();
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: false, 0.01f);
		Vector3 hitPoint = gunResult.Hit.point;
		if (gunResult.IsActive && gunResult.IsShooting && gunResult.CanFire && hitPoint != Vector3.zero)
		{
			GunController.MarkFired(GunId);
			string entityName = GameNetworkUtilities.BuildableEntityNames[_entityNameIndex];
			if (GameNetworkUtilities.EntityTypeIdsByName.ContainsKey(entityName))
			{
				GameNetworkUtilities.SpawnGameEntity(
					GameNetworkUtilities.EntityTypeIdsByName[entityName], hitPoint, Random.rotation);
			}
			_entityNameIndex = (_entityNameIndex + 1) % GameNetworkUtilities.BuildableEntityNames.Length;
		}
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
	}
}
