// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.FlashingSphere
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Flashing Sphere", "Super Infection/Casual [MASTERCLIENT]", "Rapidly spawns flashing sphere.", false, 29, ModType.Toggle, false)]
internal class FlashingSphere : MonoBehaviour
{
	private const float SpawnInterval = 0.07f;
	private const float DistanceInFrontOfPlayer = 5f;
	private const int EntitiesPerType = 10;

	private static readonly string[] EntityNames =
	{
		"SI_Resource_WeirdGear",
		"SI_Resource_BouncySand",
		"SI_Resource_FloppyMetal",
		"SI_Resource_VibratingSpring"
	};

	private float _lastSpawnTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.IsSuperInfectionMode() || !PhotonNetwork.IsMasterClient)
		{
			return;
		}

		GameNetworkUtilities.CacheGameEntityTypeIds();
		GameNetworkUtilities.DestroyAllGameEntities();
		if (Time.time <= _lastSpawnTime + SpawnInterval)
		{
			return;
		}

		_lastSpawnTime = Time.time;
		Transform head = GTPlayer.Instance.headCollider.transform;
		Vector3 center = head.position + head.forward * DistanceInFrontOfPlayer;
		List<GameEntityCreateData> entities = new();

		foreach (string entityName in EntityNames)
		{
			if (!GameNetworkUtilities.EntityTypeIdsByName.TryGetValue(entityName, out int entityTypeId))
			{
				continue;
			}

			for (int entityIndex = 0; entityIndex < EntitiesPerType; entityIndex++)
			{
				Vector3 position = center + Random.onUnitSphere;
				Quaternion rotation = Quaternion.LookRotation((center - position).normalized);
				entities.Add(new GameEntityCreateData
				{
					entityTypeId = entityTypeId,
					position = position,
					rotation = rotation,
					createData = 0L
				});
			}
		}

		GameNetworkUtilities.SpawnGameEntities(entities);
	}

	private void OnDisable()
	{
		GameNetworkUtilities.DestroyAllGameEntities();
	}
}
