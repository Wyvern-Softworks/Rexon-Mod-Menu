// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashTrail
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Trail", "World", "Leaves splash trail.", false, 2, ModType.Toggle, false)]
internal class SplashTrail : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";

	private float _lastSplashTime;

	private Vector3 _lastPosition;

	private void Start()
	{
		_lastPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
	}

	private void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			Vector3 position = GorillaTagger.Instance.offlineVRRig.transform.position;
			if (Vector3.Distance(position, _lastPosition) > 0.5f && Time.time > _lastSplashTime + 0.15f)
			{
				_lastSplashTime = Time.time;
				_lastPosition = position;
				Vector3 splashPosition = position - Vector3.up * 0.3f;
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					SplashRpc, RpcTarget.All, splashPosition, Quaternion.identity, 1f, 0.5f, false, true);
				GameNetworkUtilities.FlushAndReplayNetworkTraffic();
			}
		}
	}
}
