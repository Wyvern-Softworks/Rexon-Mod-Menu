// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Room.JoinRandom
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Room;

[Mod("Join Random", "Room", "Joins a random public lobby.", false, 6, ModType.Action, false)]
internal class JoinRandom : MonoBehaviour
{
	private void OnEnable()
	{
		PhotonNetwork.JoinRandomRoom();
	}
}
