// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.WaterSplashSelf
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Water Splash Self", "World", "Splashes water at your position.", false, 19, ModType.Toggle, false)]
internal class WaterSplashSelf : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";

	private void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			Transform rigTransform = GorillaTagger.Instance.offlineVRRig.transform;
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(
				SplashRpc, RpcTarget.All, rigTransform.position, rigTransform.rotation, 1f, 900f, true, false);
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}
}
