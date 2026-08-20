// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashHelix
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Helix", "World", "Double helix splash pattern.", false, 8, ModType.Toggle, false)]
internal class SplashHelix : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.08f;
	private const float Radius = 1.5f;
	private const float AngleStep = 25f;
	private const float HeightStep = 0.12f;
	private const float MaximumHeight = 2.5f;
	private const float MaximumDistanceSquared = 9f;

	private float _lastUpdateTime;
	private float _angleDegrees;
	private float _height;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + UpdateInterval)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		Vector3 origin = GorillaTagger.Instance.offlineVRRig.transform.position;
		SpawnSplashIfNearby(origin, PointOnHelix(origin, _angleDegrees));
		SpawnSplashIfNearby(origin, PointOnHelix(origin, _angleDegrees + 180f));
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();

		_angleDegrees = (_angleDegrees + AngleStep) % 360f;
		_height += HeightStep;
		if (_height > MaximumHeight)
		{
			_height = 0f;
		}
	}

	private Vector3 PointOnHelix(Vector3 origin, float angleDegrees)
	{
		float angleRadians = angleDegrees * Mathf.Deg2Rad;
		return origin + new Vector3(
			Mathf.Cos(angleRadians) * Radius,
			_height,
			Mathf.Sin(angleRadians) * Radius);
	}

	private static void SpawnSplashIfNearby(Vector3 origin, Vector3 position)
	{
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

	private void OnDisable()
	{
		_angleDegrees = 0f;
		_height = 0f;
	}
}
