// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GadgetRain
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Gadget Rain", "Super Infection/Casual [MASTERCLIENT]", "Rains gadgets from above.", false, 34, ModType.Toggle, false)]
internal class GadgetRain : MonoBehaviour
{
	private float _lastSpawnTime;

	private float _lastCleanupTime;


	private void Update()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.IsSuperInfectionMode() || !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		GameNetworkUtilities.CacheGameEntityTypeIds();
		if (Time.time > _lastCleanupTime + 0.3f)
		{
			_lastCleanupTime = Time.time;
			GameNetworkUtilities.DestroyAllGameEntities();
		}
		if (Time.time <= _lastSpawnTime + 0.1f)
		{
			return;
		}
		_lastSpawnTime = Time.time;
		List<GameEntityCreateData> entities = new List<GameEntityCreateData>();
		Vector3 rigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
		foreach (string entityName in GameNetworkUtilities.ResourceEntityNames)
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
	}

	private void OnDisable()
	{
		GameNetworkUtilities.DestroyAllGameEntities();
	}
}
