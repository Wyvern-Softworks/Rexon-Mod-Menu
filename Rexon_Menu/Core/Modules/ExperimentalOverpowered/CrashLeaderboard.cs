// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.ExperimentalOverpowered.CrashLeaderboard
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using TMPro;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.ExperimentalOverpowered;

[Mod("Crash Leaderboard [USE ANTIBAN]", "Experimental Overpowered [D?]", "Adds CRASH button to scoreboard.", false, 14, ModType.Toggle, false)]
internal class CrashLeaderboard : MonoBehaviour
{
	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			RestoreReportButtonLabels();
			return;
		}

		ReplaceReportButtonLabels();
		foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
		{
			Player target = line.linePlayer?.GetPlayerRef();
			if (target == null ||
				target == PhotonNetwork.LocalPlayer ||
				!line.reportInProgress ||
				Main.SelectedPlayers.Contains(target))
			{
				continue;
			}

			GameNetworkUtilities.SendMalformedSplashEffect();
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
			SendViewEvent(view.ViewID, recipients);
		}
	}

	private static void SendViewEvent(int viewId, int[] recipients)
	{
		if (recipients.Length == 0)
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

	private static void ReplaceReportButtonLabels()
	{
		foreach (GorillaScoreBoard scoreboard in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			TMP_Text label = scoreboard.buttonText;
			if (label != null && label.text.Contains("REPORT"))
			{
				label.text = label.text.Replace("REPORT", "CRASH");
			}
		}
	}

	private static void RestoreReportButtonLabels()
	{
		foreach (GorillaScoreBoard scoreboard in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			TMP_Text label = scoreboard.buttonText;
			if (label != null && label.text.Contains("CRASH"))
			{
				label.text = label.text.Replace("CRASH", "REPORT");
			}
		}
	}

	private void OnDisable()
	{
		RestoreReportButtonLabels();
	}

	private void OnDestroy()
	{
		RestoreReportButtonLabels();
	}
}
