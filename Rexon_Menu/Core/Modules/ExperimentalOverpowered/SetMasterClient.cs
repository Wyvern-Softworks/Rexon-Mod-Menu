// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SetMasterClient
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Set Master Client [W?] [USE ANTIBAN]", "Experimental Overpowered [D?]", "Sets you as master.", true, 8, ModType.Toggle, false)]
internal class SetMasterClient : MonoBehaviour
{

	private void OnEnable()
	{
		const string GameModeProperty = "gameMode";
		const string ModdedModeMarker = "MODDED_";
		if (!PhotonNetwork.CurrentRoom.CustomProperties[GameModeProperty].ToString().Contains(ModdedModeMarker))
		{
			Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
		{
			GameNetworkUtilities.SendMalformedSplashEffect();
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
		}
		Object.Destroy(this);
	}
}
