// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.UnghostAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Unghost All [USE ANTIBAN]", "Experimental Overpowered [D?]", "Unghosts all players.", true, 7, ModType.Toggle, false)]
internal class UnghostAll : MonoBehaviour
{
	private void OnEnable()
	{
		if (!GhostGun.IsSupportedRoom())
		{
			Object.Destroy(this);
			return;
		}

		GameNetworkUtilities.SendMalformedSplashEffect();
		foreach (KeyValuePair<string, int> entry in GhostGun.ViewIdByUserId.ToList())
		{
			Player player = PhotonNetwork.PlayerList.FirstOrDefault(candidate => candidate.UserId == entry.Key);
			if (player != null)
			{
				GhostGun.SendViewEvent(entry.Value, new[] { player.ActorNumber });
			}
			GhostGun.ViewIdByUserId.Remove(entry.Key);
		}

		Object.Destroy(this);
	}
}
