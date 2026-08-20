// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.KickPartyMembers
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Text;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

using Random = UnityEngine.Random;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Kick Party Members [Party]", "Overpowered", "Kicks your party members.", false, 45, ModType.Action, false)]
internal class KickPartyMembers : MonoBehaviour
{
	private const string RoomCodeCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

	private void OnEnable()
	{
		try
		{
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(GenerateRandomRoomCode(5), (JoinType)4);
				PhotonNetwork.SendAllOutgoingCommands();
			}
		}
		catch (Exception)
		{
		}
		Object.Destroy(this);
	}

	private static string GenerateRandomRoomCode(int length)
	{
		StringBuilder roomCode = new StringBuilder(length);
		for (int i = 0; i < length; i++)
		{
			roomCode.Append(RoomCodeCharacters[Random.Range(0, RoomCodeCharacters.Length)]);
		}
		return roomCode.ToString();
	}
}
