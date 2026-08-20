// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.WaterWall
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Water Wall", "World", "Creates wall of splashes.", false, 18, ModType.Toggle, false)]
internal class WaterWall : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.05f;
	private const float WallDistance = 1.5f;
	private const float MaximumDistanceSquared = 9f;

	private float _lastUpdateTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + UpdateInterval)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		Transform localRig = GorillaTagger.Instance.offlineVRRig.transform;
		Vector3 wallCenter = localRig.position + localRig.forward * WallDistance;
		float horizontalOffset = Random.Range(-1f, 1f);
		float verticalOffset = Random.Range(-0.5f, 1f);
		Vector3 splashPosition = wallCenter + localRig.right * horizontalOffset + Vector3.up * verticalOffset;
		if ((localRig.position - splashPosition).sqrMagnitude >= MaximumDistanceSquared)
		{
			return;
		}

		GorillaTagger.Instance.myVRRig.GetView.SendRpc(
			SplashRpc,
			RpcTarget.All,
			splashPosition,
			Quaternion.LookRotation(-localRig.forward),
			1f,
			0.5f,
			true,
			false);
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}
}
