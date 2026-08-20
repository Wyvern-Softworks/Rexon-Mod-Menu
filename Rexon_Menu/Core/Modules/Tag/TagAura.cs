// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.TagAura
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("Tag Aura", "Tag", "Auto-tags nearest player when RT held.", false, 5, ModType.Toggle, false)]
internal class TagAura : MonoBehaviour
{
	private void Update()
	{
		if (!PhotonNetwork.InRoom || ControllerInputPoller.TriggerFloat(XRNode.RightHand) < 0.7f)
		{
			return;
		}
		VRRig nearestRig = null;
		Player nearestPlayer = null;
		float nearestDistance = float.MaxValue;
		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		if (localRig == null)
		{
			return;
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (!GameNetworkUtilities.CanLocalPlayerTag(player))
			{
				continue;
			}
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig != null)
			{
				float distance = Vector3.Distance(localRig.transform.position, rig.transform.position);
				if (distance < nearestDistance && distance <= 4f)
				{
					nearestDistance = distance;
					nearestRig = rig;
					nearestPlayer = player;
				}
			}
		}
		if (nearestRig == null || nearestPlayer == null)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			GorillaTagManager tagManager = GameNetworkUtilities.GetTagManager();
			if (tagManager != null)
			{
				GorillaGameManager.instance.AddLastTagged((NetPlayer)nearestPlayer, (NetPlayer)PhotonNetwork.LocalPlayer);
				tagManager.AddInfectedPlayer((NetPlayer)nearestPlayer, true);
				GameNetworkUtilities.VibrateHand(isLeftHand: false, 0.5f, 0.2f);
			}
		}
		else
		{
			Vector3 originalPosition = VRRig.LocalRig.transform.position;
			VRRig.LocalRig.transform.position = nearestRig.transform.position;
			GameNetworkUtilities.SendSpoofedBodyPosition(nearestRig.transform.position, new int[1] { PhotonNetwork.MasterClient.ActorNumber });
			GameMode.ReportTag(nearestRig.Creator);
			VRRig.LocalRig.transform.position = originalPosition;
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
			GameNetworkUtilities.VibrateHand(isLeftHand: false, 0.5f, 0.2f);
		}
	}
}
