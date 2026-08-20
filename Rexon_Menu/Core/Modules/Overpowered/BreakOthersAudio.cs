// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.BreakOthersAudio
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Break Audio Nearby V1", "Overpowered", "Corrupts audio for nearby players.", false, 24, ModType.Toggle, false)]
internal class BreakOthersAudio : MonoBehaviour
{
	private const string PlayHandTapRpc = "RPC_PlayHandTap";

	private float _lastBurstTime;

	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time >= _lastBurstTime + 0.015f)
		{
			_lastBurstTime = Time.time;
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(PlayHandTapRpc, RpcTarget.Others, 111, true, 6560f);
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}
}
