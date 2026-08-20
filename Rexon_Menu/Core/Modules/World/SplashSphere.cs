// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashSphere
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Sphere", "World", "Fibonacci sphere splash pattern.", false, 7, ModType.Toggle, false)]
internal class SplashSphere : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.1f;
	private const float SphereRadius = 2f;
	private const float MaximumDistanceSquared = 9f;
	private const int PointCount = 32;

	private float _lastUpdateTime;
	private int _pointIndex;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + UpdateInterval)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		Vector3 origin = GorillaTagger.Instance.offlineVRRig.transform.position;
		Vector3 position = origin + GetFibonacciSpherePoint(_pointIndex, PointCount) * SphereRadius;
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

		_pointIndex = (_pointIndex + 1) % PointCount;
	}

	private static Vector3 GetFibonacciSpherePoint(int pointIndex, int pointCount)
	{
		float goldenRatio = (1f + Mathf.Sqrt(5f)) / 2f;
		float azimuth = Mathf.PI * 2f * pointIndex / goldenRatio;
		float polarAngle = Mathf.Acos(1f - 2f * (pointIndex + 0.5f) / pointCount);
		return new Vector3(
			Mathf.Cos(azimuth) * Mathf.Sin(polarAngle),
			Mathf.Cos(polarAngle),
			Mathf.Sin(azimuth) * Mathf.Sin(polarAngle));
	}

	private void OnDisable()
	{
		_pointIndex = 0;
	}
}
