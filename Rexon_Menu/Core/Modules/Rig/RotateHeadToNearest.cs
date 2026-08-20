// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.RotateHeadToNearest
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Rotate Head To Nearest", "Rig", "Head tracks nearest player.", false, 35, ModType.Toggle, false)]
internal class RotateHeadToNearest : MonoBehaviour
{
	private void Update()
	{
		if (GorillaGameManager.instance == null || NetworkSystem.Instance == null)
		{
			return;
		}
		VRRig localRig = GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer);
		if (localRig == null)
		{
			return;
		}
		VRRig nearestRig = null;
		float nearestDistance = float.MaxValue;
		foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
		{
			VRRig rig = GorillaGameManager.instance.FindPlayerVRRig(player);
			if (rig != null)
			{
				float distance = Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, rig.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestRig = rig;
				}
			}
		}
		if (nearestRig != null)
		{
			Vector3 direction = nearestRig.transform.position - localRig.transform.position;
			Quaternion worldRotation = Quaternion.LookRotation(direction.normalized);
			Quaternion localRotation = Quaternion.Inverse(localRig.transform.rotation) * worldRotation;
			Vector3 rotationOffset = localRotation.eulerAngles;
			rotationOffset.z = 0f;
			localRig.head.trackingRotationOffset = rotationOffset;
		}
	}

	private void OnDisable()
	{
		RestoreHeadTracking();
	}

	private void RestoreHeadTracking()
	{
		VRRig localRig = GorillaTagger.Instance?.offlineVRRig;
		if (localRig == null)
		{
			return;
		}
		if (localRig.head != null)
		{
			localRig.head.trackingRotationOffset = Vector3.zero;
		}
	}
}
