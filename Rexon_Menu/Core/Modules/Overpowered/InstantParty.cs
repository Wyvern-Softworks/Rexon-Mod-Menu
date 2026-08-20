// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.InstantParty
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Instant Party", "Overpowered", "Forms party instantly.", false, 14, ModType.Toggle, false)]
internal class InstantParty : MonoBehaviour
{
	private const float FormationDelaySeconds = 4f;
	private const string PartyFormedRpc = "PartyFormedSuccessfully";

	private int _targetActorNumber = -1;

	private float _targetDetectedAt = -1f;

	private bool _partyFormed;

	private void OnDisable()
	{
		ResetTarget();
	}

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ResetTarget();
			return;
		}

		FriendshipGroupDetection group = FriendshipGroupDetection.Instance;
		if (group == null || group.playersInProvisionalGroup == null || group.playersInProvisionalGroup.Count < 2)
		{
			ResetTarget();
			return;
		}

		int localActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		if (group.playersInProvisionalGroup[0] != localActorNumber)
		{
			return;
		}

		int targetActorNumber = FindFirstOtherActor(group.playersInProvisionalGroup, localActorNumber);
		if (targetActorNumber < 0)
		{
			ResetTarget();
			return;
		}

		if (targetActorNumber != _targetActorNumber)
		{
			_targetActorNumber = targetActorNumber;
			_targetDetectedAt = Time.time;
			_partyFormed = false;
		}

		if (_partyFormed || Time.time - _targetDetectedAt < FormationDelaySeconds)
		{
			return;
		}

		Player targetPlayer = FindPlayer(targetActorNumber);
		if (targetPlayer == null)
		{
			return;
		}

		FormParty(group, targetPlayer);
		_partyFormed = true;
	}

	private static int FindFirstOtherActor(List<int> provisionalActors, int localActorNumber)
	{
		foreach (int actorNumber in provisionalActors)
		{
			if (actorNumber != localActorNumber)
			{
				return actorNumber;
			}
		}

		return -1;
	}

	private static Player FindPlayer(int actorNumber)
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (player.ActorNumber == actorNumber)
			{
				return player;
			}
		}

		return null;
	}

	private static void FormParty(FriendshipGroupDetection group, Player targetPlayer)
	{
		int localActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		int[] partyActorNumbers = { localActorNumber, targetPlayer.ActorNumber };
		short packedBraceletColor = PackBraceletColor(Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f));
		string gameMode = ((GameModeType)Enum.Parse(
			typeof(GameModeType),
			GorillaComputer.instance.currentGameMode.Value,
			true)).ToString();

		if (!group.partyMergeIDs.ContainsKey(targetPlayer.ActorNumber))
		{
			group.partyMergeIDs[targetPlayer.ActorNumber] = partyActorNumbers;
		}

		((NetworkSceneObject)group).photonView.SendRpc(
			PartyFormedRpc,
			targetPlayer,
			gameMode,
			packedBraceletColor,
			partyActorNumbers,
			false);
		PhotonNetwork.SendAllOutgoingCommands();

		group.myPartyMemberIDs = new List<string>
		{
			PhotonNetwork.LocalPlayer.UserId,
			targetPlayer.UserId
		};
		group.myBraceletColor = UnpackBraceletColor(packedBraceletColor);
		group.myPartyMembersHash.Clear();
		group.myPartyMembersHash.Add(PhotonNetwork.LocalPlayer.UserId);
		group.myPartyMembersHash.Add(targetPlayer.UserId);
		group.OnPartyMembershipChanged();
		GorillaTagger.Instance.offlineVRRig.UpdateFriendshipBracelet();
	}

	private static short PackBraceletColor(Color color)
	{
		int red = Mathf.RoundToInt(color.r * 9f);
		int green = Mathf.RoundToInt(color.g * 9f);
		int blue = Mathf.RoundToInt(color.b * 9f);
		return (short)(red + green * 10 + blue * 100);
	}

	private static Color UnpackBraceletColor(short packedColor)
	{
		return new Color(
			(float)(packedColor % 10) / 9f,
			(float)(packedColor / 10 % 10) / 9f,
			(float)(packedColor / 100 % 10) / 9f);
	}

	private void ResetTarget()
	{
		_targetActorNumber = -1;
		_targetDetectedAt = -1f;
		_partyFormed = false;
	}
}
