// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Block.DrawGunBlock
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Reflection;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Rexon_Menu.Core.Modules.Block;

[Mod("Draw Gun [MASTER ONLY] [RG + RT] [Block Map]", "Block Mods", "Draw with building blocks. Requires master.", false, 5, ModType.Toggle, false)]
internal class DrawGunBlock : MonoBehaviour
{
	private const int PieceType = -566818631;
	private const string PieceCreatedRpc = "PieceCreatedByShelfRPC";
	private const string CreatePlayerTableMethod = "CreatePlayerTableInit";
	private const string DroppedPieceLimitField = "DROPPED_PIECE_LIMIT";

	private bool _playerTablesInitialized;
	private static float _lastPieceRequest;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.AreBuilderComponentsAvailable())
		{
			return;
		}

		if (!ControllerInputPoller.instance.rightGrab)
		{
			_playerTablesInitialized = false;
			return;
		}

		Vector3 downward = -GorillaTagger.Instance.rightHandTransform.up;
		Vector3 aimDirection = Vector3.Lerp(downward, GorillaTagger.Instance.rightHandTransform.forward, 0.5f);
		Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, aimDirection, out RaycastHit hit);
		ShowAimMarker(hit.point);

		if (ControllerInputPoller.TriggerFloat(XRNode.RightHand) <= 0.6f)
		{
			_playerTablesInitialized = false;
			return;
		}

		if (!PhotonNetwork.IsMasterClient)
		{
			RequestPiece(PieceType, hit.point, Random.rotation, 0);
			_lastPieceRequest = Time.time - 100f;
			RequestPiece(PieceType, hit.point, Random.rotation, 0);
			return;
		}

		CreatePieceAsMaster(hit.point);
	}

	private static void ShowAimMarker(Vector3 position)
	{
		GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		marker.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		marker.GetComponent<Renderer>().material.color = ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.6f
			? new Color(0.3529f, 0.0078f, 0.4078f)
			: new Color(0.7019f, 0.0118f, 0.8196f);
		marker.transform.position = position;

		Object.Destroy(marker.GetComponent<BoxCollider>());
		Object.Destroy(marker.GetComponent<Rigidbody>());
		Object.Destroy(marker.GetComponent<Collider>());
		Object.Destroy(marker, Time.deltaTime);
	}

	private static void RequestPiece(int pieceType, Vector3 position, Quaternion rotation, int materialIndex)
	{
		GameNetworkUtilities.CacheBuilderComponents();
		BuilderTable table = GameNetworkUtilities.CachedBuilderTable;
		if (table == null)
		{
			return;
		}

		float cooldown = MakeBlockModsSlower.Enabled ? 0.1f : 0.05f;
		if (Time.time <= _lastPieceRequest + cooldown)
		{
			return;
		}

		_lastPieceRequest = Time.time;
		table.RequestCreatePiece(pieceType, position, rotation, materialIndex);
	}

	private void CreatePieceAsMaster(Vector3 position)
	{
		GameNetworkUtilities.CacheBuilderComponents();
		BuilderTable table = GameNetworkUtilities.CachedBuilderTable;
		BuilderTableNetworking networking = GameNetworkUtilities.CachedBuilderNetworking;
		if (table == null || networking == null)
		{
			return;
		}

		Traverse.Create(table).Field(DroppedPieceLimitField).SetValue(int.MaxValue);
		if (!_playerTablesInitialized)
		{
			_playerTablesInitialized = true;
			MethodInfo initializePlayerTable = typeof(BuilderTableNetworking).GetMethod(
				CreatePlayerTableMethod,
				BindingFlags.Instance | BindingFlags.NonPublic);

			if (initializePlayerTable != null)
			{
				foreach (Player player in PhotonNetwork.PlayerList)
				{
					try
					{
						initializePlayerTable.Invoke(networking, new object[] { player });
					}
					catch (Exception)
					{
					}
				}
			}
		}

		int pieceId = table.CreatePieceId();
		long packedPosition = BitPackUtils.PackWorldPosForNetwork(position);
		int packedRotation = BitPackUtils.PackQuaternionForNetwork(Random.rotation);
		networking.photonView.SendRpc(
			PieceCreatedRpc,
			RpcTarget.All,
			PieceType,
			pieceId,
			packedPosition,
			packedRotation,
			0,
			(byte)4,
			0,
			PhotonNetwork.LocalPlayer);
	}

	private void OnDisable()
	{
		_playerTablesInitialized = false;
	}
}
