// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.OpenUnlockRoom
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Open/Unlock Room", "Experimental Overpowered [D?]", "Opens room to new joins.", true, 2, ModType.Toggle, false)]
internal class OpenUnlockRoom : MonoBehaviour
{
	private const string TemporaryNickname = "update";
	private const string PlatformProperty = "platform";
	private const string ClosedProperty = "closed";

	private void OnEnable()
	{
		if (!PhotonNetwork.InRoom)
		{
			Object.Destroy(this);
			return;
		}

		Hashtable roomProperties = new();
		roomProperties[(byte)253] = true;
		roomProperties[(byte)254] = true;
		roomProperties[(byte)235] = -1;
		roomProperties[(byte)236] = 0;
		roomProperties[(byte)249] = false;
		roomProperties[PlatformProperty] = "OTHER";
		roomProperties[ClosedProperty] = null;

		Dictionary<byte, object> operationParameters = new()
		{
			{ 251, roomProperties },
			{ 250, true },
			{ 231, null }
		};

		PhotonPeer peer = PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer;
		peer.SendOperation(252, operationParameters, SendOptions.SendReliable);

		string originalNickname = PhotonNetwork.LocalPlayer.NickName;
		PhotonNetwork.LocalPlayer.NickName = TemporaryNickname;
		PhotonNetwork.LocalPlayer.NickName = originalNickname;
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		Object.Destroy(this);
	}
}
