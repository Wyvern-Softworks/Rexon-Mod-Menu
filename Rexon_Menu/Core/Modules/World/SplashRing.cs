// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashRing
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Ring", "World", "Creates ring of splashes.", false, 3, ModType.Toggle, false)]
internal class SplashRing : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.1f;
	private const float Radius = 1.5f;
	private const float MaximumDistanceSquared = 9f;
	private const int AngleStepDegrees = 30;

	private float _lastUpdateTime;
	private int _angleDegrees;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + UpdateInterval)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		Vector3 origin = GorillaTagger.Instance.offlineVRRig.transform.position;
		float angleRadians = _angleDegrees * Mathf.Deg2Rad;
		Vector3 splashPosition = origin + new Vector3(
			Mathf.Cos(angleRadians) * Radius,
			0.2f,
			Mathf.Sin(angleRadians) * Radius);

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
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}

		_angleDegrees = (_angleDegrees + AngleStepDegrees) % 360;
	}
}
