// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Block.ClearAllBlocks
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaTagScripts;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Block;

[Mod("Clear All Blocks", "Block Mods", "Destroys all placed building blocks.", false, 6, ModType.Action, false)]
internal class ClearAllBlocks : MonoBehaviour
{
	private const string PieceDestroyedRpc = "PieceDestroyedRPC";

	private float _lastClearTime;

	private BuilderPiece[] _cachedBuilderPieces;

	private float _lastBlockScanTime;

	private void OnEnable()
	{
		if (Time.time <= _lastClearTime + 3f)
		{
			return;
		}
		_lastClearTime = Time.time;
		if (!PhotonNetwork.InRoom || !GameNetworkUtilities.AreBuilderComponentsAvailable())
		{
			return;
		}
		BuilderPiece[] builderPieces = GetCachedBuilderPieces();
		GameNetworkUtilities.CacheBuilderComponents();
		BuilderTableNetworking networking = GameNetworkUtilities.CachedBuilderNetworking;
		foreach (BuilderPiece piece in builderPieces)
		{
			if (piece != null && piece.gameObject.activeSelf && networking != null)
			{
				long packedPosition = BitPackUtils.PackWorldPosForNetwork(piece.transform.position);
				int packedRotation = BitPackUtils.PackQuaternionForNetwork(piece.transform.rotation);
				networking.photonView.SendRpc(
					PieceDestroyedRpc, RpcTarget.All, piece.pieceId, packedPosition, packedRotation, false, (short)2);
			}
		}
	}

	private BuilderPiece[] GetCachedBuilderPieces()
	{
		if (Time.time > _lastBlockScanTime + 5f || _cachedBuilderPieces == null)
		{
			_lastBlockScanTime = Time.time;
			_cachedBuilderPieces = Object.FindObjectsOfType<BuilderPiece>();
		}
		return _cachedBuilderPieces;
	}
}
