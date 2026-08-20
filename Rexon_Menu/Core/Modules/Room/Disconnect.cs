// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.Disconnect
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Disconnect", "Room", "Leaves the current lobby.", false, 0, ModType.Toggle, false)]
internal class Disconnect : MonoBehaviour
{
	private void OnEnable()
	{
		if (PhotonNetwork.InRoom)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
	}
}
