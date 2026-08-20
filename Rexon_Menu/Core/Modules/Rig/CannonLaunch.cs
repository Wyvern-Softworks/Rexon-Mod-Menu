// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.CannonLaunch
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Cannon Launch [ALL BUTTONS + GRIPS]", "Rig", "Massive launch in the direction you look.", false, 21, ModType.Toggle, false)]
internal sealed class CannonLaunch : MonoBehaviour
{
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float LaunchSpeed = 45f;
	private const float CooldownSeconds = 1.5f;

	private float _nextLaunchTime;
	private Rigidbody _playerBody;

	private void Start()
	{
		_playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (GorillaTagger.Instance == null ||
			GorillaTagger.Instance.offlineVRRig == null ||
			_playerBody == null ||
			!ControllerInputPoller.instance.leftControllerSecondaryButton ||
			!ControllerInputPoller.instance.rightControllerSecondaryButton ||
			!ControllerInputPoller.instance.leftGrab ||
			!ControllerInputPoller.instance.rightGrab ||
			Time.time <= _nextLaunchTime)
		{
			return;
		}

		_nextLaunchTime = Time.time + CooldownSeconds;
		_playerBody.velocity = GTPlayer.Instance.headCollider.transform.forward * LaunchSpeed;

		GorillaTagger.Instance.DoVibration(XRNode.RightHand, 1f, 0.5f);
		GorillaTagger.Instance.DoVibration(XRNode.LeftHand, 1f, 0.5f);
		GorillaTagger.Instance.myVRRig.GetView.SendRpc(
			SplashRpc,
			RpcTarget.All,
			GorillaTagger.Instance.offlineVRRig.transform.position,
			GorillaTagger.Instance.offlineVRRig.transform.rotation,
			5f,
			150f,
			true,
			false);
	}

	private void OnDisable()
	{
		_nextLaunchTime = 0f;
	}
}
