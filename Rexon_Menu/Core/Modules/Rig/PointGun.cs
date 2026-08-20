// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.PointGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Point Gun", "Rig", "Point at other players.", false, 43, ModType.Toggle, false)]
internal class PointGun : MonoBehaviour
{
	private const string GunId = "PointGun";

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: true, 0f, allowSingleTargetLock: true);
		if (gunResult.IsActive && gunResult.IsShooting && gunResult.Target != null)
		{
			VRRig targetRig = RigUtilities.GetRig(gunResult.Target);
			if (targetRig != null)
			{
				VRRig localRig = GorillaTagger.Instance.offlineVRRig;
				localRig.enabled = false;
				Quaternion targetRotation = Quaternion.LookRotation(targetRig.transform.position - localRig.transform.position);
				Quaternion rotationDelta = Quaternion.Inverse(localRig.transform.rotation) * targetRotation;
				localRig.transform.rotation *= rotationDelta;
				localRig.head.rigTarget.rotation = localRig.transform.rotation * rotationDelta;
			}
		}
	}

	private void OnDisable()
	{
		ResetPointerGun();
	}

	private void ResetPointerGun()
	{
		GunController.Release(GunId);
		if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}
}
