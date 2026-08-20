// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.GripWallWalk
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Wall Walk", "Rig", "Walk on walls with grip.", false, 26, ModType.Toggle, false)]
internal sealed class GripWallWalk : MonoBehaviour
{
	private Vector3 _contactPoint;
	private Vector3 _contactNormal;

	private void Update()
	{
		if (GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false))
		{
			RaycastHit handHit = GTPlayer.Instance.lastHitInfoHand;
			_contactPoint = handHit.point;
			_contactNormal = handHit.normal;
		}

		if (_contactPoint != Vector3.zero && ControllerInputPoller.instance.rightGrab)
		{
			GorillaTagger.Instance.rigidbody.AddForce(_contactNormal * -9.81f, (ForceMode)5);
			Physics.gravity = Vector3.zero;
		}
		else if (!ControllerInputPoller.instance.rightGrab)
		{
			_contactPoint = Vector3.zero;
		}
	}

	private void OnDisable()
	{
		_contactPoint = Vector3.zero;
		Physics.gravity = Vector3.down * 9.81f;
	}
}
