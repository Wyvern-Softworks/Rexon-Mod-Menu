// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashWave
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Wave", "World", "Expanding ring splash.", false, 9, ModType.Toggle, false)]
internal class SplashWave : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.08f;
	private const float RadiusStep = 0.16f;
	private const float MinimumRadius = 0.3f;
	private const float MaximumRadius = 3f;
	private const float MaximumDistanceSquared = 9f;

	private float _lastUpdateTime;
	private float _radius;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + UpdateInterval)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		_radius += RadiusStep;
		if (_radius > MaximumRadius)
		{
			_radius = MinimumRadius;
		}

		Vector3 origin = GorillaTagger.Instance.offlineVRRig.transform.position;
		int splashCount = Mathf.Max(4, (int)(_radius * 4f));
		for (int splashIndex = 0; splashIndex < splashCount; splashIndex++)
		{
			float angleRadians = 360f / splashCount * splashIndex * Mathf.Deg2Rad;
			Vector3 position = origin + new Vector3(
				Mathf.Cos(angleRadians) * _radius,
				0.1f,
				Mathf.Sin(angleRadians) * _radius);
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
			}
		}

		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}

	private void OnDisable()
	{
		_radius = 0f;
	}
}
