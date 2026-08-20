// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ConnectEU
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Connect EU", "Room", "Connect to EU servers.", true, 4, ModType.Toggle, false)]
internal class ConnectEU : MonoBehaviour
{

	private void OnEnable()
	{
		PhotonNetwork.Disconnect();
		PhotonNetwork.ConnectToRegion("eu");
		Object.Destroy(this);
	}
}
