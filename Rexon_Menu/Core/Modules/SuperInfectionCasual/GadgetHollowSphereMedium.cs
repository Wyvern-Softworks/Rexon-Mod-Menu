// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GadgetHollowSphereMedium
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Gadget Hollow Sphere Medium", "Super Infection/Casual [MASTERCLIENT]", "Spawns medium hollow sphere.", true, 26, ModType.Toggle, false)]
internal class GadgetHollowSphereMedium : MonoBehaviour
{

	private void OnEnable()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.IsSuperInfectionMode() || !PhotonNetwork.IsMasterClient)
		{
			Object.Destroy(this);
			return;
		}
		GameNetworkUtilities.CacheGameEntityTypeIds();
		Vector3 center = GTPlayer.Instance.headCollider.transform.position + GTPlayer.Instance.headCollider.transform.forward * 5f;
		List<GameEntityCreateData> entities = new();
		string[] entityNames = GameNetworkUtilities.BuildableEntityNames;
		foreach (string entityName in entityNames)
		{
			if (GameNetworkUtilities.EntityTypeIdsByName.ContainsKey(entityName))
			{
				int entityTypeId = GameNetworkUtilities.EntityTypeIdsByName[entityName];
				for (int entityIndex = 0; entityIndex < 100; entityIndex++)
				{
					Vector3 entityPosition = center + Random.onUnitSphere * 2f;
					Vector3 directionToCenter = center - entityPosition;
					Quaternion rotation = Quaternion.LookRotation(directionToCenter.normalized);
					entities.Add(new GameEntityCreateData
					{
						entityTypeId = entityTypeId,
						position = entityPosition,
						rotation = rotation,
						createData = 0L
					});
				}
			}
		}
		GameNetworkUtilities.SpawnGameEntities(entities);
		Object.Destroy(this);
	}
}

