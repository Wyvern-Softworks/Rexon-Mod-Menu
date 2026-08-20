// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Room.JoinRandomGhostCode
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaNetworking;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Room;

[Mod("Join Random Ghost Code", "Room", "Cycles through ghost room codes.", false, 7, ModType.Toggle, false)]
internal sealed class JoinRandomGhostCode : MonoBehaviour
{
	private static readonly string[] GhostRoomCodes =
	{
		"DAISY", "DAISY09", "GHOST", "SREN17", "SREN18", "BANSHEE", "BANJO",
		"SPIDER", "ECHO", "HUNT", "ISEEYOU", "PBBV", "PBBC", "RUN", "HUNT",
		"J3VU", "HIDE", "HELP", "Chippd", "666", "UNKNOWN"
	};

	private float _lastJoinAttemptAt;
	private int _nextCodeIndex;

	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null || Time.time <= _lastJoinAttemptAt + 10f)
		{
			return;
		}

		_lastJoinAttemptAt = Time.time;
		_nextCodeIndex++;
		if (_nextCodeIndex >= GhostRoomCodes.Length)
		{
			_nextCodeIndex = 0;
		}

		PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(
			GhostRoomCodes[_nextCodeIndex],
			(JoinType)0);
	}

	private void OnDisable()
	{
		_lastJoinAttemptAt = 0f;
		_nextCodeIndex = 0;
	}
}
