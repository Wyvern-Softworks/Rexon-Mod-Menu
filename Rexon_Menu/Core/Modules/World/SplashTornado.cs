// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashTornado
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Tornado", "World", "Spiral splash pattern.", false, 5, ModType.Toggle, false)]
internal class SplashTornado : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.08f;
	private const float Radius = 2f;
	private const float AngleStep = 30f;
	private const float HeightStep = 0.15f;
	private const float MaximumHeight = 3f;
	private const float MaximumDistanceSquared = 16f;

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
		float angleRadians = _angleDegrees * Mathf.Deg2Rad;
		Vector3 position = origin + new Vector3(
			Mathf.Cos(angleRadians) * Radius,
			_height,
			Mathf.Sin(angleRadians) * Radius);

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

		_angleDegrees = (_angleDegrees + AngleStep) % 360f;
		_height += HeightStep;
		if (_height > MaximumHeight)
		{
			_height = 0f;
		}
	}

	private void OnDisable()
	{
		_angleDegrees = 0f;
		_height = 0f;
	}
}
