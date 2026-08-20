// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.SplashMatrix
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Splash Matrix", "World", "Grid of splashes falls from above.", false, 12, ModType.Toggle, false)]
internal class SplashMatrix : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.06f;
	private const float GridSpacing = 0.8f;
	private const float StartingHeight = 2f;
	private const float FallStep = 0.3f;
	private const float MaximumFallDistance = 2f;
	private const float MaximumDistanceSquared = 16f;
	private const int GridCellCount = 9;

	private float _lastUpdateTime;
	private int _gridCellIndex;
	private float _fallDistance;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + UpdateInterval)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		Vector3 origin = GorillaTagger.Instance.offlineVRRig.transform.position;
		int row = _gridCellIndex / 3;
		float xOffset = (_gridCellIndex % 3 - 1) * GridSpacing;
		float zOffset = (row - 1) * GridSpacing;
		Vector3 position = origin + new Vector3(xOffset, StartingHeight - _fallDistance, zOffset);

		if ((origin - position).sqrMagnitude < MaximumDistanceSquared)
		{
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(
				SplashRpc,
				RpcTarget.All,
				position,
				Quaternion.identity,
				1f,
				0.5f,
				false,
				true);
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}

		_fallDistance += FallStep;
		if (_fallDistance > MaximumFallDistance)
		{
			_fallDistance = 0f;
			_gridCellIndex = (_gridCellIndex + 1) % GridCellCount;
		}
	}

	private void OnDisable()
	{
		ResetPattern();
	}

	private void OnDestroy()
	{
		ResetPattern();
	}

	private void ResetPattern()
	{
		_gridCellIndex = 0;
		_fallDistance = 0f;
	}
}
