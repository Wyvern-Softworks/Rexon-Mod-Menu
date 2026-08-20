// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.SplashSpam
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Splash Spam [RT]", "Rig", "Spam splash effects from your hand.", false, 42, ModType.Toggle, false)]
internal class SplashSpam : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";

	private float _lastSplashTime;


	private void Update()
	{
		if (ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.7f && Time.time > _lastSplashTime)
		{
			_lastSplashTime = Time.time + 0.1f;
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(
				SplashRpc, RpcTarget.All, GorillaTagger.Instance.rightHandTransform.position,
				GorillaTagger.Instance.rightHandTransform.rotation, 3f, 50f, true, false);
		}
	}

	private void OnDisable()
	{
		_lastSplashTime = 0f;
	}
}
