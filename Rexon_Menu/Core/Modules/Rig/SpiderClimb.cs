// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.SpiderClimb
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Spider Climb", "Rig", "Climb walls with grips.", false, 28, ModType.Toggle, false)]
internal sealed class SpiderClimb : MonoBehaviour
{
	private bool _isClimbing;
	private Vector3 _surfaceNormal;
	private Rigidbody _playerBody;

	private void Start()
	{
		_playerBody = GameNetworkUtilities.GetPlayerRigidbody();
	}

	private void Update()
	{
		if (_playerBody == null)
		{
			return;
		}

		bool bothGripsHeld = ControllerInputPoller.instance.leftGrab
			&& ControllerInputPoller.instance.rightGrab;
		if (!bothGripsHeld)
		{
			if (_isClimbing)
			{
				_isClimbing = false;
				_playerBody.useGravity = true;
			}
			return;
		}

		Vector3 bodyPosition = GTPlayer.Instance.bodyCollider.transform.position;
		Vector3 movementDirection = GameNetworkUtilities.GetHeadForward();
		bool foundSurface = Physics.Raycast(bodyPosition, movementDirection, out RaycastHit hit, 0.5f)
			|| Physics.Raycast(bodyPosition, -_surfaceNormal, out hit, 0.5f);

		if (foundSurface)
		{
			_isClimbing = true;
			_surfaceNormal = hit.normal;
			_playerBody.useGravity = false;
			_playerBody.velocity = Vector3.ProjectOnPlane(movementDirection, _surfaceNormal).normalized * 3f;
		}
		else if (_isClimbing)
		{
			_playerBody.useGravity = false;
			_playerBody.velocity = Vector3.zero;
		}
	}

	private void OnDisable()
	{
		_isClimbing = false;
		if (_playerBody != null)
		{
			_playerBody.useGravity = true;
		}
	}
}
