// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.LagAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Lag All", "Overpowered", "Lags all players.", false, 12, ModType.Toggle, false)]
internal class LagAll : MonoBehaviour
{
	private float _lastBurstTime;

	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time >= _lastBurstTime + 2.5f)
		{
			_lastBurstTime = Time.time;
			for (int i = 0; i < Rexon_Menu.Core.Modules.Settings.LagPower.Power; i++)
			{
				PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)186, (object)new object[1] { float.NaN }, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)0
				}, SendOptions.SendUnreliable);
			}
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}
}
