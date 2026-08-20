// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Block.FreezeGunBlock
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Rexon_Menu.Core.Modules.Block;

[Mod("Freeze Gun [MASTER ONLY] [Block Map]", "Block Mods", "Freeze targeted player, master needed", false, 4, ModType.Toggle, false)]
internal class FreezeGunBlock : MonoBehaviour
{
	private const string GunId = "FreezeGunBlock";
	private const int BlockTypeId = -566818631;
	private const string PieceCreatedRpc = "PieceCreatedByShelfRPC";

	private readonly List<Player> _targets = new List<Player>();
	private float _lastTargetToggleTime;
	private float _lastTargetRemovalTime;
	private bool _hasInitializedPlayerTable;
	private GameObject _targetMarker;
	private static float _lastBlockCreateTime;

	private void OnDisable()
	{
		_targets.Clear();
		_hasInitializedPlayerTable = false;
		DestroyTargetMarker();
	}

	private void Update()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.AreBuilderComponentsAvailable())
		{
			return;
		}

		GunController.GunResult gunResult = GunController.GetGunResult(
			GunId,
			targetPlayers: true,
			0f,
			allowSingleTargetLock: true);
		if (gunResult.IsActive
			&& gunResult.IsShooting
			&& gunResult.Target != null
			&& gunResult.Target != PhotonNetwork.LocalPlayer
			&& Time.time > _lastTargetToggleTime + 1f)
		{
			_lastTargetToggleTime = Time.time;
			if (_targets.Count == 0)
			{
				_hasInitializedPlayerTable = false;
				_targets.Add(gunResult.Target);
			}
		}

		RaycastHit hit = gunResult.Hit;
		if (gunResult.IsActive
			&& gunResult.IsShooting
			&& hit.collider != null)
		{
			RemoveTargetNearHitPoint(hit.point);
		}
		if (_targets.Count > 0)
		{
			FreezeSelectedPlayer();
		}
		_targets.RemoveAll(player => !PhotonNetwork.PlayerListOthers.Contains(player));
	}

	private void FreezeSelectedPlayer()
	{
		Player targetPlayer = _targets.FirstOrDefault();
		if (targetPlayer == null)
		{
			return;
		}
		VRRig targetRig = RigUtilities.GetRig(targetPlayer);
		if (targetRig == null)
		{
			return;
		}

		Vector3 targetHeadPosition = targetRig.headMesh.transform.position;
		FlashTargetMarker(targetRig.transform.position);
		if (!PhotonNetwork.IsMasterClient)
		{
			RequestCreateBlock(BlockTypeId, targetHeadPosition, Random.rotation, 0);
			_lastBlockCreateTime = Time.time - 100f;
			RequestCreateBlock(BlockTypeId, targetHeadPosition, Random.rotation, 0);
			return;
		}

		GameNetworkUtilities.CacheBuilderComponents();
		BuilderTable builderTable = GameNetworkUtilities.CachedBuilderTable;
		BuilderTableNetworking builderNetworking = GameNetworkUtilities.CachedBuilderNetworking;
		if (builderTable == null
			|| builderNetworking == null)
		{
			return;
		}

		Traverse.Create(builderTable)
			.Field("DROPPED_PIECE_LIMIT")
			.SetValue(int.MaxValue);
		if (!_hasInitializedPlayerTable)
		{
			_hasInitializedPlayerTable = true;
			MethodInfo initializePlayerTable = typeof(BuilderTableNetworking).GetMethod(
				"CreatePlayerTableInit",
				BindingFlags.Instance | BindingFlags.NonPublic);
			initializePlayerTable?.Invoke(builderNetworking, new object[] { targetPlayer });
		}

		for (int blockIndex = 0; blockIndex < 2; blockIndex++)
		{
			Vector3 randomOffset = new Vector3(
				Random.Range(-0.6f, 0.6f),
				Random.Range(-0.6f, 0.6f),
				Random.Range(-0.6f, 0.6f));
			long packedPosition = BitPackUtils.PackWorldPosForNetwork(targetHeadPosition + randomOffset);
			int packedRotation = BitPackUtils.PackQuaternionForNetwork(Random.rotation);
			int pieceId = builderTable.CreatePieceId();
			builderNetworking.photonView.SendRpc(
				PieceCreatedRpc,
				RpcTarget.All,
				BlockTypeId,
				pieceId,
				packedPosition,
				packedRotation,
				0,
				(byte)4,
				0,
				PhotonNetwork.LocalPlayer);
		}
	}

	private void RemoveTargetNearHitPoint(Vector3 hitPoint)
	{
		if (Time.time <= _lastTargetRemovalTime + 1f)
		{
			return;
		}
		_lastTargetRemovalTime = Time.time;
		for (int index = _targets.Count - 1; index >= 0; index--)
		{
			VRRig rig = RigUtilities.GetRig(_targets[index]);
			if (rig != null
				&& Vector3.Distance(rig.transform.position, hitPoint) <= 0.6f)
			{
				_targets.RemoveAt(index);
				_hasInitializedPlayerTable = false;
			}
		}
	}

	private static void RequestCreateBlock(
		int blockTypeId,
		Vector3 position,
		Quaternion rotation,
		int materialIndex)
	{
		GameNetworkUtilities.CacheBuilderComponents();
		BuilderTable builderTable = GameNetworkUtilities.CachedBuilderTable;
		if (builderTable == null)
		{
			return;
		}

		float cooldown = MakeBlockModsSlower.Enabled ? 0.1f : 0.05f;
		if (Time.time > _lastBlockCreateTime + cooldown)
		{
			_lastBlockCreateTime = Time.time;
			builderTable.RequestCreatePiece(blockTypeId, position, rotation, materialIndex);
		}
	}

	private void FlashTargetMarker(Vector3 position)
	{
		_targetMarker = GameObject.CreatePrimitive((PrimitiveType)0);
		_targetMarker.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		_targetMarker.transform.position = position;
		Object.Destroy(_targetMarker.GetComponent<Rigidbody>());
		Object.Destroy(_targetMarker.GetComponent<Collider>());
		if (GameNetworkUtilities.XrayShader == null)
		{
			GameNetworkUtilities.XrayShader = Shader.Find("GUI/Text Shader");
		}
		Renderer renderer = _targetMarker.GetComponent<Renderer>();
		renderer.material.shader = GameNetworkUtilities.XrayShader;
		renderer.material.color = Color.blue;
		Object.Destroy(_targetMarker, Time.deltaTime);
	}

	private void DestroyTargetMarker()
	{
		if (_targetMarker != null)
		{
			Object.Destroy(_targetMarker);
			_targetMarker = null;
		}
	}

	private void OnDestroy()
	{
		GunController.Release(GunId);
	}
}
