// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.AntiTag
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Anti Tag", "Tag", "Prevents getting tagged.", false, 4, ModType.Toggle, false)]
internal class AntiTag : MonoBehaviour
{
	private float _lastSpoofTime = -1f;

	private bool _hasSpoofed;

	private bool _isEvading;


	private void Update()
	{
		if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
		{
			return;
		}
		float triggerDistance = _isEvading ? 4f : 2f;
		VRRig nearestRig = null;
		float nearestDistance = float.MaxValue;
		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		Player nearestPlayer = null;
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (!GameNetworkUtilities.CanPlayerTag(player, PhotonNetwork.LocalPlayer))
			{
				continue;
			}
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig != null)
			{
				float rightHandDistance = Vector3.Distance(localRig.transform.position, rig.rightHandTransform.position);
				float leftHandDistance = Vector3.Distance(localRig.transform.position, rig.leftHandTransform.position);
				float handDistance = Mathf.Min(rightHandDistance, leftHandDistance);
				if (handDistance < nearestDistance)
				{
					nearestDistance = handDistance;
					nearestRig = rig;
					nearestPlayer = player;
				}
			}
		}
		if (nearestRig != null && nearestPlayer != null && nearestDistance <= triggerDistance)
		{
			_isEvading = true;
			if (_lastSpoofTime < 0f)
			{
				_lastSpoofTime = Time.time;
			}
			GameNetworkUtilities.SetPhotonSerializeTickMultiplier(9999f);
			if (Time.time <= _lastSpoofTime + 0.1f && _hasSpoofed)
			{
				return;
			}
			_lastSpoofTime = Time.time;
			_hasSpoofed = true;
			GameNetworkUtilities.SendSpoofedBodyPosition(nearestRig.transform.position + new Vector3(0f, -15f, 0f), new int[1] { PhotonNetwork.MasterClient.ActorNumber });
			Vector3 localPosition = localRig.transform.position;
			foreach (Player player in PhotonNetwork.PlayerListOthers)
			{
				if (player != PhotonNetwork.MasterClient)
				{
					GameNetworkUtilities.SendSpoofedBodyPosition(localPosition, new int[1] { player.ActorNumber });
				}
			}
			PhotonNetwork.SendAllOutgoingCommands();
		}
		else if (_isEvading)
		{
			_isEvading = false;
			_lastSpoofTime = -1f;
			_hasSpoofed = false;
			GameNetworkUtilities.SetPhotonSerializeTickMultiplier(1000f);
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	private void OnDisable()
	{
		GameNetworkUtilities.SetPhotonSerializeTickMultiplier(1000f);
		_lastSpoofTime = -1f;
		_hasSpoofed = false;
		_isEvading = false;
	}
}
