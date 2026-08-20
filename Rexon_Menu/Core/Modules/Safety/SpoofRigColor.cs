// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SpoofRigColor
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Spoof Rig Color [STUMP]", "Safety", "Sends random color to others so they see you in a fake color.", false, 11, ModType.Toggle, false)]
internal class SpoofRigColor : MonoBehaviour
{
	private const string InitializeMaterialRpc = "RPC_InitializeNoobMaterial";

	internal struct SpoofData
	{
		public Player Player;

		public string UserId;

		public SpoofData(Player player, string userId)
		{
			Player = player;
			UserId = userId;
		}
	}

	private static readonly List<SpoofData> SpoofedPlayers = new List<SpoofData>();


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			RestoreRigColor();
			return;
		}
		for (int index = SpoofedPlayers.Count - 1; index >= 0; index--)
		{
			SpoofData spoofData = SpoofedPlayers[index];
			if (spoofData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(spoofData.Player))
			{
				SpoofedPlayers.RemoveAt(index);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			bool wasSpoofed = SpoofedPlayers.Any(entry => entry.UserId == player.UserId);
			if (!wasSpoofed)
			{
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					InitializeMaterialRpc, player,
					Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
				GameNetworkUtilities.FlushAndReplayNetworkTraffic();
				SpoofedPlayers.Add(new SpoofData(player, player.UserId));
			}
		}
	}

	private void RestoreRigColor()
	{
		SpoofedPlayers.Clear();
	}

	private void OnDisable()
	{
		RestoreRigColor();
	}
}
