// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.LaserEyes
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using ZoneState = GameEntityManager.ZoneState;

namespace Recovered.Obfuscated;

[Mod("Laser Eyes", "Super Infection/Casual [MASTERCLIENT]", "Shoots lasers from your eyes.", false, 28, ModType.Toggle, false)]
internal class LaserEyes : MonoBehaviour
{
	private const string CreateItemRpc = "CreateItemRPC";
	private const string LaserEntityName = "SI_Resource_VibratingSpring";
	private const float SpawnInterval = 0.1f;
	private const int SegmentsPerEye = 25;
	private const float SegmentSpacing = 0.2f;

	private static readonly Vector3 RightEyeOffset = new(0.07f, 0f, -0.2f);
	private static readonly Vector3 LeftEyeOffset = new(-0.07f, 0f, -0.2f);
	private static readonly Quaternion SegmentRotationOffset = Quaternion.Euler(90f, 0f, 0f);

	private float _lastSpawnTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.IsSuperInfectionMode() || !PhotonNetwork.IsMasterClient)
		{
			return;
		}

		GameNetworkUtilities.CacheGameEntityTypeIds();
		if (!GameNetworkUtilities.EntityTypeIdsByName.TryGetValue(LaserEntityName, out int entityTypeId))
		{
			return;
		}

		GameNetworkUtilities.DestroyAllGameEntities();
		if (Time.time <= _lastSpawnTime + SpawnInterval)
		{
			return;
		}

		_lastSpawnTime = Time.time;
		GameEntityManager entityManager = GameEntityManager.activeManager;
		if (entityManager == null)
		{
			return;
		}

		SuperInfectionManager infectionManager = SuperInfectionManager.activeSuperInfectionManager;
		if (infectionManager != null && infectionManager.zoneSuperInfection != null)
		{
			infectionManager.zoneSuperInfection.OnZoneInit();
		}

		entityManager.SetZoneState((ZoneState)3);

		Transform head = GTPlayer.Instance.headCollider.transform;
		Vector3 forward = head.forward;
		Vector3 rightEye = head.position + head.TransformDirection(RightEyeOffset);
		Vector3 leftEye = head.position + head.TransformDirection(LeftEyeOffset);
		Quaternion segmentRotation = Quaternion.LookRotation(forward) * SegmentRotationOffset;
		int invalidParentNetId = entityManager.GetNetIdFromEntityId(GameEntityId.Invalid);

		List<int> netIds = new();
		List<int> entityTypeIds = new();
		List<long> packedPositions = new();
		List<int> packedRotations = new();
		List<long> packedVelocities = new();
		List<int> parentNetIds = new();

		for (int segmentIndex = 0; segmentIndex < SegmentsPerEye; segmentIndex++)
		{
			float distance = segmentIndex * SegmentSpacing;
			AddSegment(rightEye + forward * distance);
			AddSegment(leftEye + forward * distance);
		}

		entityManager.photonView.SendRpc(
			CreateItemRpc,
			RpcTarget.All,
			netIds.ToArray(),
			entityTypeIds.ToArray(),
			packedPositions.ToArray(),
			packedRotations.ToArray(),
			packedVelocities.ToArray(),
			parentNetIds.ToArray());

		void AddSegment(Vector3 position)
		{
			int entityId = 1 + entityManager.FactoryGetBuiltInEntityCountById(entityTypeId);
			netIds.Add(entityManager.CreateNetId(entityId));
			entityTypeIds.Add(entityTypeId);
			packedPositions.Add(BitPackUtils.PackWorldPosForNetwork(position));
			packedRotations.Add(BitPackUtils.PackQuaternionForNetwork(segmentRotation));
			packedVelocities.Add(0L);
			parentNetIds.Add(invalidParentNetId);
		}
	}

	private void OnDisable()
	{
		GameNetworkUtilities.DestroyAllGameEntities();
	}
}
