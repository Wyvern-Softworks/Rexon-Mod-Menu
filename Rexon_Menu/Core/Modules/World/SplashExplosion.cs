// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.SplashExplosion
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Splash Explosion", "World", "Both grips for radial burst.", false, 6, ModType.Toggle, false)]
internal sealed class SplashExplosion : MonoBehaviour
{
	private const float CooldownSeconds = 0.3f;
	private const int SplashCount = 10;
	private const float SplashRadius = 2f;
	private const string SplashRpc = "RPC_PlaySplashEffect";

	private float _lastBurstTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom ||
			GorillaTagger.Instance == null ||
			GorillaTagger.Instance.offlineVRRig == null ||
			!ControllerInputPoller.instance.leftGrab ||
			!ControllerInputPoller.instance.rightGrab ||
			Time.time < _lastBurstTime + CooldownSeconds)
		{
			return;
		}

		_lastBurstTime = Time.time;
		Vector3 center = GorillaTagger.Instance.offlineVRRig.transform.position;

		for (int index = 0; index < SplashCount; index++)
		{
			float angle = 36f * index * Mathf.Deg2Rad;
			Vector3 position = center + new Vector3(
				Mathf.Cos(angle) * SplashRadius,
				0.2f,
				Mathf.Sin(angle) * SplashRadius);

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

		Recovered.Obfuscated.GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}
}
