// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.SpamIndexedAudio
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Spam Indexed Audio [RG + RT]", "Overpowered", "Spams selected audio index. Right grip + right trigger to fire.", false, 28, ModType.Toggle, false)]
internal class SpamIndexedAudio : MonoBehaviour
{
	private const float PlaybackInterval = 0.05f;
	private const float PlaybackVolume = 6560f;
	private const string PlayHandTapRpc = "RPC_PlayHandTap";

	private float _lastPlaybackTime;
	private bool _useAlternateHand;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
		{
			return;
		}

		if (!ControllerInputPoller.instance.rightGrab ||
			ControllerInputPoller.TriggerFloat(XRNode.RightHand) <= 0.6f ||
			Time.time < _lastPlaybackTime + PlaybackInterval)
		{
			return;
		}

		try
		{
			_lastPlaybackTime = Time.time;
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(
				PlayHandTapRpc,
				RpcTarget.All,
				AudioIndex.CurrentIndex,
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
