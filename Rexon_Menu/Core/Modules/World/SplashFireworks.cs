// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashFireworks
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Fireworks", "World", "Burst splashes around you.", false, 4, ModType.Toggle, false)]
internal class SplashFireworks : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";

	private float _lastBurstTime;


	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastBurstTime + 0.15f)
		{
			return;
		}
		_lastBurstTime = Time.time;
		Vector3 rigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
		for (int i = 0; i < 5; i++)
		{
			Vector3 splashPosition = rigPosition + Random.insideUnitSphere * 3f;
			if ((rigPosition - splashPosition).sqrMagnitude < 9f)
			{
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					SplashRpc, RpcTarget.All, splashPosition, Quaternion.identity, 1f, 0.5f, false, true);
			}
		}
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}
}
