// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.DisableOthersRigLerping
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Disable Others Rig Lerping", "Rig", "Remove rig interpolation on others.", false, 50, ModType.Toggle, false)]
internal class DisableOthersRigLerping : MonoBehaviour
{
	private void Update()
	{
		if (GorillaParent.instance == null || GorillaTagger.Instance == null)
		{
			return;
		}

		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (rig == null || rig == localRig)
			{
				continue;
			}

			rig.lerpValueBody = 1f;
			rig.lerpValueFingers = 1f;
		}
	}

	private void RestoreRigLerpValues()
	{
		if (GorillaParent.instance == null || GorillaTagger.Instance == null)
		{
			return;
		}

		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		if (localRig == null)
		{
			return;
		}

		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (rig == null || rig == localRig)
			{
				continue;
			}

			rig.lerpValueBody = localRig.lerpValueBody;
			rig.lerpValueFingers = localRig.lerpValueFingers;
		}
	}

	private void OnDisable()
	{
		RestoreRigLerpValues();
	}
}
