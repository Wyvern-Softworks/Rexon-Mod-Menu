// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.GrappleHook
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Grapple Hook", "Rig", "Right grip + trigger to grapple.", false, 24, ModType.Toggle, false)]
internal sealed class GrappleHook : MonoBehaviour
{
	private LineRenderer _line;
	private Vector3 _grapplePoint;
	private bool _grappling;
	private Rigidbody _playerBody;

	private void Start()
	{
		_playerBody = GTPlayer.Instance.bodyCollider.attachedRigidbody;
	}

	private void Update()
	{
		if (_playerBody == null)
		{
			return;
		}

		bool grappleInput = ControllerInputPoller.instance.rightGrab
			&& ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.7f;
		if (!grappleInput)
		{
			StopGrappling();
			return;
		}

		Transform hand = GorillaTagger.Instance.rightHandTransform;
		if (!_grappling)
		{
			if (!Physics.Raycast(hand.position, -hand.up, out RaycastHit hit, 100f))
			{
				return;
			}

			_grappling = true;
			_grapplePoint = hit.point;
			EnsureLine();
		}

		Vector3 toGrapple = _grapplePoint - _playerBody.transform.position;
		float distance = toGrapple.magnitude;
		_playerBody.velocity = toGrapple.normalized * Mathf.Clamp(distance * 3f, 5f, 30f);
		_playerBody.useGravity = false;

		if (_line != null)
		{
			_line.enabled = true;
			_line.SetPosition(0, hand.position);
			_line.SetPosition(1, _grapplePoint);
		}

		if (distance < 0.5f)
		{
			StopGrappling();
		}
	}

	private void EnsureLine()
	{
		if (_line != null)
		{
			return;
		}

		GameObject lineObject = new("GrappleHookLine");
		_line = lineObject.AddComponent<LineRenderer>();
		_line.positionCount = 2;
		_line.startWidth = 0.01f;
		_line.endWidth = 0.01f;
		_line.material = ShaderPatch.CreateTransparentMaterial(Color.white);
		_line.startColor = Color.cyan;
		_line.endColor = Color.cyan;
		_line.useWorldSpace = true;
	}

	private void StopGrappling()
	{
		_grappling = false;
		if (_playerBody != null)
		{
			_playerBody.useGravity = true;
		}
		if (_line != null)
		{
			_line.enabled = false;
		}
	}

	private void OnDisable()
	{
		_grappling = false;
		if (_playerBody != null)
		{
			_playerBody.useGravity = true;
		}
		if (_line != null)
		{
			Destroy(_line.gameObject);
			_line = null;
		}
	}
}
