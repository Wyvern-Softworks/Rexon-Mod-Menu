// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Movement.SlingShotFly
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Movement;

[Mod("SlingShot Fly", "Movement", "Hold right secondary to fly.", false, 5, ModType.Toggle, false)]
internal class SlingShotFly : MonoBehaviour
{
	private const float FlightSpeed = 15f;
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

		if (ControllerInputPoller.instance.rightControllerSecondaryButton)
		{
			_playerBody.velocity = GameNetworkUtilities.GetHeadForward() * FlightSpeed;
			_playerBody.useGravity = false;
		}
		else
		{
			_playerBody.useGravity = true;
		}
	}

	private void OnDisable()
	{
		if (_playerBody != null)
		{
			_playerBody.useGravity = true;
		}
	}
}
