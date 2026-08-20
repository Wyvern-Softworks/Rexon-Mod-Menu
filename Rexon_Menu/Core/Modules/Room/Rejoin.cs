// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.Rejoin
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Rejoin", "Room", "Rejoins the room you're in.", false, 1, ModType.Toggle, false)]
internal class Rejoin : MonoBehaviour
{

	private void OnEnable()
	{
		if (PhotonNetwork.InRoom)
		{
			GorillaTagger.Instance.StartCoroutine(RejoinRoom());
		}
	}

	private IEnumerator RejoinRoom()
	{
		string roomName = PhotonNetwork.CurrentRoom.Name;
		PhotonNetwork.Disconnect();
		yield return new WaitForSeconds(3f);
		((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(roomName, (JoinType)0);
	}
}
