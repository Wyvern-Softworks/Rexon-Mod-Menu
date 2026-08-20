// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.PlatformWallWalk
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Platform Wall Walk [RIGHT GRIP]", "Rig", "Spawn a platform to walk on walls.", false, 29, ModType.Toggle, false)]
internal sealed class PlatformWallWalk : MonoBehaviour
{
	private static readonly Vector3 PlatformScale = new(0.0125f, 0.28f, 0.3825f);

	private Vector3 _contactPoint;
	private Vector3 _contactNormal;
	private GameObject _platform;

	private void Update()
	{
		if (!ControllerInputPoller.instance.rightGrab)
		{
			ResetState();
			return;
		}

		if (GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false))
		{
			RaycastHit handHit = GTPlayer.Instance.lastHitInfoHand;
			_contactPoint = handHit.point;
			_contactNormal = handHit.normal;
		}

		if (_contactPoint == Vector3.zero
			|| Vector3.Distance(_contactPoint, GorillaTagger.Instance.rightHandTransform.position) >= 0.7f)
		{
			DestroyPlatform();
			return;
		}

		if (_platform == null)
		{
			_platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_platform.GetComponent<Renderer>().material.color = new Color(0.54f, 0.17f, 0.89f);
			_platform.transform.localScale = PlatformScale;
		}

		_platform.transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, -0.0075f, 0f);
		_platform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(_contactNormal * -5f, (ForceMode)5);
		GTPlayer.Instance.GetComponent<Rigidbody>().useGravity = false;
	}

	private void ResetState()
	{
		_contactPoint = Vector3.zero;
		DestroyPlatform();
		if (GTPlayer.Instance != null)
		{
			GTPlayer.Instance.GetComponent<Rigidbody>().useGravity = true;
		}
	}

	private void DestroyPlatform()
	{
		if (_platform != null)
		{
			Destroy(_platform);
			_platform = null;
		}
	}

	private void OnDisable()
	{
		ResetState();
	}

	private void OnDestroy()
	{
		ResetState();
	}
}
