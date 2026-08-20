// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashAura
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Aura", "World", "Splashes around you.", false, 1, ModType.Toggle, false)]
internal class SplashAura : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";

	private float _lastSplashTime;


	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time >= _lastSplashTime + 0.1f)
		{
			_lastSplashTime = Time.time;
			Vector3 rigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
			Vector3 randomOffset = new Vector3(
				Random.Range(-1.5f, 1.5f), Random.Range(0f, 0.5f), Random.Range(-1.5f, 1.5f));
			Vector3 splashPosition = rigPosition + randomOffset;
			if ((rigPosition - splashPosition).sqrMagnitude < 9f)
			{
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					SplashRpc, RpcTarget.All, splashPosition, Quaternion.identity, 1f, 0.5f, false, true);
				GameNetworkUtilities.FlushAndReplayNetworkTraffic();
			}
		}
	}
}
