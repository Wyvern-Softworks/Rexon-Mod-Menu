// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.SlingshotLaunch
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Slingshot Launch [BOTH GRIPS]", "Rig", "Hold to charge, release to launch forward.", false, 22, ModType.Toggle, false)]
internal sealed class SlingshotLaunch : MonoBehaviour
{
	private const string SplashEffectRpc = "RPC_PlaySplashEffect";

	private bool _charging;
	private float _chargeSeconds;
	private Vector3 _launchDirection;
	private Rigidbody _playerBody;

	private void Start()
	{
		_playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (_playerBody == null)
		{
			return;
		}

		bool bothGripsHeld = ControllerInputPoller.instance.leftGrab
			&& ControllerInputPoller.instance.rightGrab;
		if (bothGripsHeld)
		{
			if (!_charging)
			{
				_charging = true;
				_chargeSeconds = 0f;
			}

			_chargeSeconds = Mathf.Clamp(_chargeSeconds + Time.deltaTime, 0f, 3f);
			_launchDirection = GTPlayer.Instance.headCollider.transform.forward;
			float vibrationStrength = _chargeSeconds / 3f * 0.5f;
			GorillaTagger.Instance.DoVibration(XRNode.LeftHand, vibrationStrength, Time.deltaTime);
			GorillaTagger.Instance.DoVibration(XRNode.RightHand, vibrationStrength, Time.deltaTime);
			return;
		}

		if (_charging)
		{
			Launch();
		}
	}

	private void Launch()
	{
		_playerBody.velocity = _launchDirection * _chargeSeconds * 15f;
		GorillaTagger.Instance.DoVibration(XRNode.LeftHand, 1f, 0.3f);
		GorillaTagger.Instance.DoVibration(XRNode.RightHand, 1f, 0.3f);
		GorillaTagger.Instance.myVRRig.GetView.SendRpc(
			SplashEffectRpc,
			RpcTarget.All,
			GorillaTagger.Instance.offlineVRRig.transform.position,
			GorillaTagger.Instance.offlineVRRig.transform.rotation,
			_chargeSeconds * 2f,
			100f,
			true,
			false);
		ResetCharge();
	}

	private void ResetCharge()
	{
		_charging = false;
		_chargeSeconds = 0f;
	}

	private void OnDisable()
	{
		ResetCharge();
	}

	private void OnDestroy()
	{
		ResetCharge();
	}
}
