// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.HandTapSpam
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Hand Tap Spam", "World", "Spams hand tap sounds.", false, 23, ModType.Toggle, false)]
internal class HandTapSpam : MonoBehaviour
{
	private const string PlayHandTapRpc = "RPC_PlayHandTap";

	private float _lastTapTime;

	private bool _leftHand;


	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time >= _lastTapTime + 0.05f)
		{
			_lastTapTime = Time.time;
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(PlayHandTapRpc, RpcTarget.All, 66, _leftHand, 0.5f);
			_leftHand = !_leftHand;
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}
}
