// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SpazMonke
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Spaz Monke", "Rig", "Makes your rig spaz out.", false, 5, ModType.Toggle, false)]
internal class SpazMonke : MonoBehaviour
{

	private void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig localRig = GorillaTagger.Instance.offlineVRRig;
			if (localRig != null)
			{
				localRig.headBodyOffset = new Vector3(
					Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
			}
		}
	}

	private void OnDisable()
	{
		RestoreRigPose();
	}

	private void RestoreRigPose()
	{
		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		if (localRig != null)
		{
			localRig.headBodyOffset = Vector3.zero;
		}
	}
}
