// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.AntibanCrashGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Crash Gun [USE ANTIBAN]", "Experimental Overpowered [D?]", "Crashes target.", false, 12, ModType.Toggle, false)]
public class AntibanCrashGun : MonoBehaviour
{
	private const string GunId = "CrashGunDETECTED";

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			return;
		}
		if (!IsSupportedRoom())
		{
			return;
		}

		GunController.GunResult gun = GunController.GetGunResult(GunId, targetPlayers: true, 0.5f);
		if (!gun.IsActive || !gun.CanFire || !gun.IsShooting || gun.Target == null)
		{
			return;
		}

		GunController.MarkFired(GunId);
		GameNetworkUtilities.SendMalformedSplashEffect();
		Main.SelectedPlayers.Add(gun.Target);
		VRRig targetRig = RigUtilities.GetRig(gun.Target);
		PhotonView targetView = targetRig != null ? RigUtilities.GetPhotonView(targetRig) : null;
		if (targetView == null || targetView.Owner == null)
		{
			return;
		}

		int[] recipients = PhotonNetwork.PlayerList
			.Where(player => player.ActorNumber != gun.Target.ActorNumber)
			.Select(player => player.ActorNumber)
			.ToArray();
		if (recipients.Length == 0)
		{
			return;
		}

		Hashtable payload = new Hashtable { { (byte)0, targetView.ViewID } };
		PhotonNetwork.NetworkingClient.OpRaiseEvent(
			204,
			payload,
			new RaiseEventOptions { TargetActors = recipients },
			SendOptions.SendReliable);
	}

	private static bool IsSupportedRoom()
	{
		object gameMode = PhotonNetwork.CurrentRoom?.CustomProperties["gameMode"];
		return gameMode != null && gameMode.ToString().Contains("MODDED_");
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
	}
}
