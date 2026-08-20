// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Tag.TagAssistNearestPlayer
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Tag;

[Mod("Tag Assist Nearest Player [RT]", "Tag", "Pulls you toward the closest taggable player.", false, 6, ModType.Toggle, false)]
internal class TagAssistNearestPlayer : MonoBehaviour
{

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		VRRig nearestRig = null;
		float nearestDistance = float.MaxValue;
		VRRig localRig = GorillaGameManager.instance.FindPlayerVRRig((NetPlayer)PhotonNetwork.LocalPlayer);
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (!GorillaGameManager.instance.LocalCanTag((NetPlayer)PhotonNetwork.LocalPlayer, (NetPlayer)player))
			{
				continue;
			}
			VRRig rig = GorillaGameManager.instance.FindPlayerVRRig((NetPlayer)player);
			if (rig != null && localRig != null)
			{
				float distance = Vector3.Distance(localRig.transform.position, rig.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestRig = rig;
				}
			}
		}
		if (ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.1f && nearestRig != null)
		{
			MoveTowardRig(nearestRig);
		}
	}

	private void MoveTowardRig(VRRig targetRig)
	{
		Vector3 displacement = targetRig.transform.position - new Vector3(0f, 1.3f, 0f) - GTPlayer.Instance.transform.position;
		displacement = displacement / displacement.magnitude * Mathf.Min(2f / 45f, displacement.magnitude);
		GTPlayer.Instance.transform.position += displacement;
	}
}
