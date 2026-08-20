// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GhostGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Ghost Gun [USE ANTIBAN]", "Experimental Overpowered [D?]", "Ghosts target.", false, 5, ModType.Toggle, false)]
internal class GhostGun : MonoBehaviour
{
	private const string GunId = "GhostGunDETECTED";

	internal static readonly Dictionary<string, int> ViewIdByUserId =
		new Dictionary<string, int>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ViewIdByUserId.Clear();
			GunController.Release(GunId);
			return;
		}
		if (!IsSupportedRoom())
		{
			return;
		}

		RemoveDepartedPlayers();
		GunController.GunResult gun = GunController.GetGunResult(GunId, targetPlayers: true, 0.5f);
		if (!gun.IsActive || !gun.CanFire || !gun.IsShooting || gun.Target == null)
		{
			return;
		}

		GunController.MarkFired(GunId);
		GameNetworkUtilities.SendMalformedSplashEffect();
		string userId = gun.Target.UserId;
		if (ViewIdByUserId.TryGetValue(userId, out int existingViewId))
		{
			SendViewEvent(existingViewId, new[] { gun.Target.ActorNumber });
			ViewIdByUserId.Remove(userId);
			return;
		}

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
		SendViewEvent(targetView.ViewID, recipients);
		ViewIdByUserId[userId] = targetView.ViewID;
	}

	internal static void SendViewEvent(int viewId, int[] recipients)
	{
		if (recipients == null || recipients.Length == 0)
		{
			return;
		}

		Hashtable payload = new Hashtable { { (byte)0, viewId } };
		PhotonNetwork.NetworkingClient.OpRaiseEvent(
			204,
			payload,
			new RaiseEventOptions { TargetActors = recipients },
			SendOptions.SendReliable);
	}

	internal static bool IsSupportedRoom()
	{
		object gameMode = PhotonNetwork.CurrentRoom?.CustomProperties["gameMode"];
		return gameMode != null && gameMode.ToString().Contains("MODDED_");
	}

	private static void RemoveDepartedPlayers()
	{
		HashSet<string> activeUserIds = new HashSet<string>(
			PhotonNetwork.PlayerList
				.Where(player => player.UserId != null)
				.Select(player => player.UserId));
		foreach (string userId in ViewIdByUserId.Keys.Where(id => !activeUserIds.Contains(id)).ToList())
		{
			ViewIdByUserId.Remove(userId);
		}
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
	}
}
