// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GhostAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Linq;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Ghost All [USE ANTIBAN]", "Experimental Overpowered [D?]", "Ghosts all players.", true, 6, ModType.Toggle, false)]
internal class GhostAll : MonoBehaviour
{
	private void OnEnable()
	{
		if (!GhostGun.IsSupportedRoom())
		{
			Object.Destroy(this);
			return;
		}

		GameNetworkUtilities.SendMalformedSplashEffect();
		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}

			PhotonView view = RigUtilities.GetPhotonView(rig);
			if (view == null || view.Owner == null || GhostGun.ViewIdByUserId.ContainsKey(view.Owner.UserId))
			{
				continue;
			}

			int[] recipients = PhotonNetwork.PlayerList
				.Where(player => player.ActorNumber != view.Owner.ActorNumber)
				.Select(player => player.ActorNumber)
				.ToArray();
			GhostGun.SendViewEvent(view.ViewID, recipients);
			GhostGun.ViewIdByUserId[view.Owner.UserId] = view.ViewID;
		}

		Object.Destroy(this);
	}
}
