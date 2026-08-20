// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.LoudHandTaps
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Loud Hand Taps", "Rig", "Hand taps play at max volume.", false, 40, ModType.Toggle, false)]
internal class LoudHandTaps : MonoBehaviour
{
	private const string PlayHandTapRpc = "RPC_PlayHandTap";

	private float _lastTapTime;


	private void Update()
	{
		if (GorillaTagger.Instance != null && GorillaTagger.Instance.myVRRig != null && Time.time > _lastTapTime + 0.1f)
		{
			bool leftTapped = GorillaTagger.Instance.lastLeftTap != 0f && Time.time - GorillaTagger.Instance.lastLeftTap < 0.15f;
			bool rightTapped = GorillaTagger.Instance.lastRightTap != 0f && Time.time - GorillaTagger.Instance.lastRightTap < 0.15f;
			if (leftTapped || rightTapped)
			{
				_lastTapTime = Time.time;
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(PlayHandTapRpc, RpcTarget.All, 0, leftTapped, float.MaxValue);
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(PlayHandTapRpc, RpcTarget.All, 0, leftTapped, float.MaxValue);
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(PlayHandTapRpc, RpcTarget.All, 0, leftTapped, float.MaxValue);
			}
		}
	}
}
