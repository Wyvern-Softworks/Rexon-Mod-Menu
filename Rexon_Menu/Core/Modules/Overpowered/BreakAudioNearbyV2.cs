// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.BreakAudioNearbyV2
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Break Audio Nearby V2", "Overpowered", "Corrupts audio for nearby players. Right grip + trigger.", false, 25, ModType.Toggle, false)]
internal class BreakAudioNearbyV2 : MonoBehaviour
{
	private const string PlayHandTapRpc = "RPC_PlayHandTap";
	private const int AudioIndex = 213;
	private const float PlaybackVolume = 6560f;

	private bool _useAlternateHand;

	private void Update()
	{
		if (!PhotonNetwork.InRoom ||
			GorillaGameManager.instance == null ||
			!ControllerInputPoller.instance.rightGrab ||
			ControllerInputPoller.TriggerFloat(XRNode.RightHand) <= 0.6f)
		{
			return;
		}

		try
		{
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(
				PlayHandTapRpc,
				RpcTarget.Others,
				AudioIndex,
				!_useAlternateHand,
				PlaybackVolume);
			_useAlternateHand = !_useAlternateHand;
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
		catch (Exception)
		{
		}
	}
}
