// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.SplashVortex
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Splash Vortex", "World", "Splashes spiral around you.", false, 11, ModType.Toggle, false)]
internal class SplashVortex : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float UpdateInterval = 0.08f;
	private const float AngleStep = 35f;
	private const float RadiusStep = 0.05f;
	private const float MinimumRadius = 0.3f;
	private const float MaximumRadius = 2.5f;
	private const float MaximumDistanceSquared = 16f;

	private float _lastUpdateTime;
	private float _angleDegrees;
	private float _radius;

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
			Mathf.Cos(angleRadians) * _radius,
			0.2f,
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
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}

		_angleDegrees += AngleStep;
		_radius += RadiusStep;
		if (_radius > MaximumRadius)
		{
			_radius = MinimumRadius;
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
		_angleDegrees = 0f;
		_radius = MinimumRadius;
	}
}
