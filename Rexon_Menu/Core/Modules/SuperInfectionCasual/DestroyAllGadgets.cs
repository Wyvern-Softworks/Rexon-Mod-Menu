// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.DestroyAllGadgets
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Destroy All Gadgets", "Super Infection/Casual [MASTERCLIENT]", "Destroys all spawned gadgets.", true, 36, ModType.Toggle, false)]
internal class DestroyAllGadgets : MonoBehaviour
{
	private void OnEnable()
	{
		if (PhotonNetwork.InRoom && GameNetworkUtilities.IsSuperInfectionMode())
		{
			GameNetworkUtilities.DestroyAllGameEntities();
		}
		Object.Destroy(this);
	}
}
