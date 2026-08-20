// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.KickPartyMembersV2
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Kick Party Members V2", "Overpowered", "Kicks party members.", false, 46, ModType.Toggle, false)]
internal class KickPartyMembersV2 : MonoBehaviour
{
	private void OnEnable()
	{
		if (FriendshipGroupDetection.Instance.IsInParty)
		{
			GTPlayer.Instance.StartCoroutine(KickPartyCoroutine());
		}
		Object.Destroy(this);
	}

	private static IEnumerator KickPartyCoroutine()
	{
		const string photonHandlerObjectName = "PhotonMono";
		const string nextSendTickField = "nextSendTickCountOnSerialize";
		const string requestPartyGameModeRpc = "RequestPartyGameMode";
		const string infectionGameMode = "Infection";

		PhotonHandler photonHandler = GameObject.Find(photonHandlerObjectName).GetComponent<PhotonHandler>();
		if (photonHandler != null)
		{
			Traverse.Create((object)photonHandler).Field(nextSendTickField).SetValue((object)(int)(Time.realtimeSinceStartup * 9999f));
		}

		yield return new WaitForSeconds(3f);

		if (FriendshipGroupDetection.Instance.IsInParty)
		{
			List<int> partyActorNumbers = new List<int>();
			foreach (string partyMemberId in FriendshipGroupDetection.Instance.myPartyMemberIDs)
			{
				if (partyMemberId == PhotonNetwork.LocalPlayer.UserId)
				{
					continue;
				}

				foreach (Player player in PhotonNetwork.PlayerList)
				{
					if (player.UserId == partyMemberId)
					{
						partyActorNumbers.Add(player.ActorNumber);
						break;
					}
				}
			}

			for (int request = 0; request < 3907; request++)
			{
				((NetworkSceneObject)FriendshipGroupDetection.Instance).photonView.SendRpc(requestPartyGameModeRpc, partyActorNumbers.ToArray(), infectionGameMode);
			}
			PhotonNetwork.SendAllOutgoingCommands();
		}

		yield return new WaitForSeconds(5f);
		if (photonHandler != null)
		{
			Traverse.Create((object)photonHandler).Field(nextSendTickField).SetValue((object)(int)(Time.realtimeSinceStartup * 1000f));
		}
	}
}
