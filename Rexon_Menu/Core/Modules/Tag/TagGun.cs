// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.TagGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Tag Gun", "Tag", "Tags player you shoot.", false, 1, ModType.Toggle, false)]
internal class TagGun : MonoBehaviour
{
	private const string GunId = "TagGun";
	private const float GunCooldown = 0.5f;

	private bool _tagInProgress;
	private Coroutine _tagCoroutine;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			return;
		}

		GunController.GunResult gun = GunController.GetGunResult(GunId, targetPlayers: true, GunCooldown);
		if (!gun.IsActive || !gun.IsShooting || !gun.CanFire || gun.Target == null || _tagInProgress)
		{
			return;
		}

		GunController.MarkFired(GunId);
		if (PhotonNetwork.IsMasterClient)
		{
			TagPlayerAsMaster(gun.Target);
			return;
		}

		if (!GameNetworkUtilities.CanLocalPlayerTag(gun.Target))
		{
			return;
		}

		VRRig targetRig = MatBridge.GetVRRigFor(gun.Target);
		if (targetRig != null && _tagCoroutine == null)
		{
			_tagCoroutine = StartCoroutine(TagPlayerAsClient(targetRig));
		}
	}

	private static void TagPlayerAsMaster(Player targetPlayer)
	{
		GorillaTagManager tagManager = GameNetworkUtilities.GetTagManager();
		if (tagManager == null)
		{
			return;
		}

		Player taggingPlayer = null;
		foreach (Player candidate in PhotonNetwork.PlayerList)
		{
			if (GameNetworkUtilities.CanPlayerTag(candidate, targetPlayer))
			{
				taggingPlayer = candidate;
				break;
			}
		}

		if (taggingPlayer != null && targetPlayer != taggingPlayer)
		{
			GorillaGameManager.instance.AddLastTagged((NetPlayer)targetPlayer, (NetPlayer)taggingPlayer);
		}
		tagManager.AddInfectedPlayer((NetPlayer)targetPlayer, true);
		GameNetworkUtilities.VibrateHand(isLeftHand: false, 0.5f, 0.2f);
	}

	private IEnumerator TagPlayerAsClient(VRRig targetRig)
	{
		_tagInProgress = true;
		Vector3 originalPosition = VRRig.LocalRig.transform.position;
		Vector3 targetPosition = targetRig.transform.position;
		VRRig.LocalRig.transform.position = targetPosition;
		GameNetworkUtilities.SendSpoofedBodyPosition(
			targetPosition,
			new[] { PhotonNetwork.MasterClient.ActorNumber });
		GameMode.ReportTag(targetRig.Creator);
		VRRig.LocalRig.transform.position = originalPosition;
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		GameNetworkUtilities.VibrateHand(isLeftHand: false, 0.5f, 0.2f);
		_tagInProgress = false;
		_tagCoroutine = null;
		yield break;
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
		_tagInProgress = false;
		if (_tagCoroutine != null)
		{
			StopCoroutine(_tagCoroutine);
			_tagCoroutine = null;
		}
	}
}
