// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Movement.Fly
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Movement;

[Mod("Fly", "Movement", "Hold right secondary to fly.", false, 4, ModType.Toggle, false)]
internal sealed class Fly : MonoBehaviour
{
	private Rigidbody _playerBody;
	private bool _wasFlying;

	private void Update()
	{
		if (_playerBody == null && GTPlayer.Instance != null)
		{
			_playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();
		}

		if (_playerBody == null)
		{
			return;
		}

		if (ControllerInputPoller.instance.rightControllerSecondaryButton)
		{
			_playerBody.isKinematic = true;
			GTPlayer.Instance.transform.position += GTPlayer.Instance.headCollider.transform.forward * 15f * Time.deltaTime;
			_wasFlying = true;
		}
		else if (_wasFlying)
		{
			RestorePhysics();
		}
	}

	private void RestorePhysics()
	{
		if (_playerBody != null)
		{
			_playerBody.isKinematic = false;
			_playerBody.velocity = Vector3.zero;
			_playerBody.useGravity = true;
		}

		_wasFlying = false;
	}

	private void OnDisable()
	{
		RestorePhysics();
	}
}
