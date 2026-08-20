// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ConnectUS
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Connect US", "Room", "Connect to US servers.", true, 2, ModType.Toggle, false)]
internal class ConnectUS : MonoBehaviour
{
	private void OnEnable()
	{
		PhotonNetwork.Disconnect();
		PhotonNetwork.ConnectToRegion("us");
		Object.Destroy(this);
	}
}
