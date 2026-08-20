// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.WallRun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Wall Run [RIGHT GRIP]", "Rig", "Run along walls when near them.", false, 27, ModType.Toggle, false)]
internal sealed class WallRun : MonoBehaviour
{
	private const float MaximumRunDuration = 2f;

	private bool _running;
	private Vector3 _wallNormal;
	private float _runStartedAt;
	private Rigidbody _playerBody;

	private void Start()
	{
		_playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (_playerBody == null || !ControllerInputPoller.instance.rightGrab)
		{
			_running = false;
			return;
		}

		Transform hand = GorillaTagger.Instance.rightHandTransform;
		if (!Physics.Raycast(hand.position, hand.right, out RaycastHit wallHit, 1f))
		{
			_running = false;
			return;
		}

		if (!_running)
		{
			_running = true;
			_wallNormal = wallHit.normal;
			_runStartedAt = Time.time;
		}

		if (Time.time >= _runStartedAt + MaximumRunDuration)
		{
			return;
		}

		Vector3 wallTangent = Vector3.Cross(_wallNormal, Vector3.up);
		if (Vector3.Dot(wallTangent, GTPlayer.Instance.headCollider.transform.forward) < 0f)
		{
			wallTangent = -wallTangent;
		}

		_playerBody.velocity = wallTangent * 8f + Vector3.up * 2f;
		_playerBody.velocity -= _wallNormal * 2f;
	}

	private void OnDisable()
	{
		_running = false;
	}

	private void OnDestroy()
	{
		_running = false;
	}
}
