// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GadgetAura
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Gadget Aura", "Super Infection/Casual [MASTERCLIENT]", "Spawns gadgets around you.", true, 33, ModType.Toggle, false)]
internal class GadgetAura : MonoBehaviour
{
	private void OnEnable()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.IsSuperInfectionMode() || !PhotonNetwork.IsMasterClient)
		{
			Object.Destroy(this);
			return;
		}
		GameNetworkUtilities.CacheGameEntityTypeIds();
		List<GameEntityCreateData> entities = new List<GameEntityCreateData>();
		Vector3 rigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
		foreach (string entityName in GameNetworkUtilities.BuildableEntityNames)
		{
			if (GameNetworkUtilities.EntityTypeIdsByName.TryGetValue(entityName, out int entityTypeId))
			{
				for (int copyIndex = 0; copyIndex < 100; copyIndex++)
				{
					Vector3 spawnPosition = new Vector3(
						rigPosition.x + Random.Range(-15f, 15f),
						rigPosition.y + Random.Range(0f, 15f),
						rigPosition.z + Random.Range(-15f, 15f));
					entities.Add(new GameEntityCreateData
					{
						entityTypeId = entityTypeId,
						position = spawnPosition,
						rotation = Random.rotation,
						createData = 0L
					});
				}
			}
		}
		GameNetworkUtilities.SpawnGameEntities(entities);
		Object.Destroy(this);
	}
}
