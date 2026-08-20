// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashPulse
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Pulse", "World", "Pulsing ring of splashes.", false, 10, ModType.Toggle, false)]
internal class SplashPulse : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.1f;
	private const float PhaseSpeed = 3f;
	private const int SplashCount = 8;
	private const float MaximumDistanceSquared = 9f;

	private float _lastUpdateTime;
	private float _phase;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + UpdateInterval)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		_phase += Time.deltaTime * PhaseSpeed;
		float radius = (Mathf.Sin(_phase) + 1f) * 1.25f + 0.3f;
		Vector3 origin = GorillaTagger.Instance.offlineVRRig.transform.position;
		for (int splashIndex = 0; splashIndex < SplashCount; splashIndex++)
		{
			float angleRadians = 360f / SplashCount * splashIndex * Mathf.Deg2Rad;
			Vector3 splashPosition = origin + new Vector3(
				Mathf.Cos(angleRadians) * radius,
				0.1f,
				Mathf.Sin(angleRadians) * radius);
			if ((origin - splashPosition).sqrMagnitude < MaximumDistanceSquared)
			{
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					SplashRpc,
					RpcTarget.All,
					splashPosition,
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
		_phase = 0f;
	}
}
