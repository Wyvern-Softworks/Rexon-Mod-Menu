// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.DashForward
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Dash Forward", "Rig", "Left secondary to dash forward.", false, 19, ModType.Toggle, false)]
internal sealed class DashForward : MonoBehaviour
{
	private const float CooldownSeconds = 0.5f;
	private const float DashSpeed = 15f;

	private float _lastDashAt;
	private Rigidbody _playerBody;

	private void Start()
	{
		_playerBody = Recovered.Obfuscated.GameNetworkUtilities.GetPlayerRigidbody();
	}

	private void Update()
	{
		if (_playerBody == null
			|| !ControllerInputPoller.instance.leftControllerSecondaryButton
			|| Time.time <= _lastDashAt + CooldownSeconds)
		{
			return;
		}

		_lastDashAt = Time.time;
		_playerBody.velocity = Recovered.Obfuscated.GameNetworkUtilities.GetHeadForward() * DashSpeed;
	}

	private void OnDisable()
	{
		_lastDashAt = 0f;
	}
}
