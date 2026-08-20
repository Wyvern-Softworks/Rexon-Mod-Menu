// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.TagSelf
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using Rexon_Menu_Mat;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Tag Self", "Tag", "Gets yourself tagged.", false, 3, ModType.Toggle, false)]
internal class TagSelf : MonoBehaviour
{
	private float _lastSpoofTime = -1f;
	private bool _hasSpoofed;
	private bool _isActive;

	private void OnEnable()
	{
		if (!PhotonNetwork.InRoom || MatBridge.IsInfected(PhotonNetwork.LocalPlayer))
		{
			Finish();
			return;
		}

		_lastSpoofTime = -1f;
		_hasSpoofed = false;
		_isActive = true;
	}

	private void Update()
	{
		if (!_isActive)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			Finish();
			return;
		}
		if (MatBridge.IsInfected(PhotonNetwork.LocalPlayer))
		{
			Finish();
			return;
		}

		if (PhotonNetwork.IsMasterClient)
		{
			TagLocalPlayerAsMasterClient();
			Finish();
			return;
		}

		SpoofLocalRigNearNearestTaggablePlayer();
	}

	private static void TagLocalPlayerAsMasterClient()
	{
		Player tagger = null;
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (GameNetworkUtilities.CanPlayerTag(player, PhotonNetwork.LocalPlayer))
			{
				tagger = player;
				break;
			}
		}

		if (tagger != null && tagger != PhotonNetwork.LocalPlayer)
		{
			GorillaGameManager.instance.AddLastTagged(
				(NetPlayer)PhotonNetwork.LocalPlayer,
				(NetPlayer)tagger);
		}

		GorillaTagManager tagManager = GameNetworkUtilities.GetTagManager();
		if (tagManager != null)
		{
			tagManager.AddInfectedPlayer((NetPlayer)PhotonNetwork.LocalPlayer, true);
		}
	}

	private void SpoofLocalRigNearNearestTaggablePlayer()
	{
		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		VRRig nearestRig = null;
		Player nearestPlayer = null;
		float nearestDistance = float.MaxValue;
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (!GameNetworkUtilities.CanPlayerTag(player, PhotonNetwork.LocalPlayer))
			{
				continue;
			}

			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null)
			{
				continue;
			}

			float distance = Vector3.Distance(localRig.transform.position, rig.transform.position);
			if (distance < nearestDistance)
			{
				nearestDistance = distance;
				nearestRig = rig;
				nearestPlayer = player;
			}
		}
		if (nearestRig == null || nearestPlayer == null)
		{
			return;
		}

		if (_lastSpoofTime < 0f)
		{
			_lastSpoofTime = Time.time;
		}
		if (_hasSpoofed && Time.time <= _lastSpoofTime + 0.1f)
		{
			return;
		}

		_lastSpoofTime = Time.time;
		_hasSpoofed = true;
		GameNetworkUtilities.SetPhotonSerializeTickMultiplier(9999f);
		Vector3 spoofedPosition = nearestRig.rightHandTransform.position + new Vector3(0f, 0.4f, 0f);
		GameNetworkUtilities.SendSpoofedBodyPosition(
			spoofedPosition,
			new[] { PhotonNetwork.MasterClient.ActorNumber });
		GameNetworkUtilities.SendSpoofedBodyPosition(
			spoofedPosition,
			new[] { nearestPlayer.ActorNumber });

		Vector3 realPosition = localRig.transform.position;
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (player != PhotonNetwork.MasterClient && player != nearestPlayer)
			{
				GameNetworkUtilities.SendSpoofedBodyPosition(realPosition, new[] { player.ActorNumber });
			}
		}
		PhotonNetwork.SendAllOutgoingCommands();
	}

	private void Finish()
	{
		ResetState();
		Object.Destroy(this);
		BundleManager.RefreshMenu();
		ConfigurationManager.SaveIfAutoLoadEnabled();
	}

	private void ResetState()
	{
		GameNetworkUtilities.SetPhotonSerializeTickMultiplier(1000f);
		_lastSpoofTime = -1f;
		_hasSpoofed = false;
		_isActive = false;
	}

	private void OnDisable()
	{
		ResetState();
	}
}
