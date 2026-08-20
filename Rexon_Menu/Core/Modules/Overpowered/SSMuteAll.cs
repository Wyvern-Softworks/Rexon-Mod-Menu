// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SSMuteAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("SS Mute All", "Overpowered", "Mutes all players.", false, 15, ModType.Toggle, false)]
internal class SSMuteAll : MonoBehaviour
{
	private const byte VoiceEventCode = 202;
	private const int EventsPerFrame = 2;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}

		PhotonVoiceNetwork voiceNetwork = PhotonVoiceNetwork.Instance;
		if (voiceNetwork == null || ((VoiceConnection)voiceNetwork).Client == null)
		{
			return;
		}

		for (int eventIndex = 0; eventIndex < EventsPerFrame; eventIndex++)
		{
			Dictionary<byte, object> voiceFrame = new()
			{
				{ 1, 67 },
				{ 2, 0 },
				{ 3, 0 },
				{ 4, 0 },
				{ 5, 0 },
				{ 10, null },
				{ 11, (byte)0 },
				{ 12, (Codec)11 }
			};
			object[] payload = { (byte)0, (byte)1, new object[] { voiceFrame } };
			RaiseEventOptions options = new() { Receivers = (ReceiverGroup)1 };
			SendOptions sendOptions = new() { Reliability = false, Channel = 0 };
			LoadBalancingTransport voiceClient = ((VoiceConnection)voiceNetwork).Client;
			((LoadBalancingClient)voiceClient).OpRaiseEvent(VoiceEventCode, payload, options, sendOptions);
		}

		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}
}
