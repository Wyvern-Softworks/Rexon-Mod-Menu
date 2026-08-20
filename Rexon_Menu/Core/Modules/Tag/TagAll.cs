// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.TagAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Tag All", "Tag", "Tags all players.", true, 2, ModType.Toggle, false)]
internal class TagAll : MonoBehaviour
{
	private void OnEnable()
	{
		if (!PhotonNetwork.InRoom)
		{
			Object.Destroy(this);
			return;
		}

		if (PhotonNetwork.IsMasterClient)
		{
			TagAllAsMasterClient();
		}
		else
		{
			TagAllAsInfectedClient();
		}

		Object.Destroy(this);
	}

	private static void TagAllAsMasterClient()
	{
		GorillaTagManager tagManager = GameNetworkUtilities.GetTagManager();
		if (tagManager == null)
		{
			return;
		}

		Player taggingPlayer = FindEligibleTagger();
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (taggingPlayer != null && player != taggingPlayer)
			{
				GorillaGameManager.instance.AddLastTagged((NetPlayer)player, (NetPlayer)taggingPlayer);
			}
			tagManager.AddInfectedPlayer((NetPlayer)player, true);
		}
	}

	private static Player FindEligibleTagger()
	{
		foreach (Player candidate in PhotonNetwork.PlayerList)
		{
			foreach (Player target in PhotonNetwork.PlayerList)
			{
				if (GameNetworkUtilities.CanPlayerTag(candidate, target))
				{
					return candidate;
				}
			}
		}

		return null;
	}

	private void TagAllAsInfectedClient()
	{
		if (!MatBridge.IsInfected(PhotonNetwork.LocalPlayer))
		{
			return;
		}

		Vector3 originalPosition = VRRig.LocalRig.transform.position;
		foreach (VRRig rig in VRRigCache.ActiveRigs.Where(IsUninfectedRemoteRig))
		{
			Vector3 targetPosition = rig.transform.position;
			VRRig.LocalRig.transform.position = targetPosition;
			GameNetworkUtilities.SendSpoofedBodyPosition(
				targetPosition,
				new[] { PhotonNetwork.MasterClient.ActorNumber });
			GameMode.ReportTag(rig.Creator);
		}

		VRRig.LocalRig.transform.position = originalPosition;
		GameNetworkUtilities.SendSpoofedBodyPosition(
			originalPosition,
			new[] { PhotonNetwork.MasterClient.ActorNumber });
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		GameNetworkUtilities.VibrateHand(isLeftHand: false, 0.5f, 0.2f);
	}

	private static bool IsInfectedOrInvalid(VRRig rig)
	{
		return rig == null ||
			rig.Creator == null ||
			MatBridge.IsInfected(rig.Creator.GetPlayerRef());
	}

	private static bool IsUninfectedRemoteRig(VRRig rig)
	{
		return rig != null &&
			rig != VRRig.LocalRig &&
			!IsInfectedOrInvalid(rig);
	}
}
