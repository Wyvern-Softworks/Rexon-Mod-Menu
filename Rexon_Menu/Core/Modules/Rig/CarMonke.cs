// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CarMonke
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("Car Monke", "Rig", "Triggers to surf forward.", false, 25, ModType.Toggle, false)]
internal class CarMonke : MonoBehaviour
{
	private Rigidbody _rigidbody;

	private void Start()
	{
		_rigidbody = GameNetworkUtilities.GetPlayerRigidbody();
	}

	private void Update()
	{
		if (_rigidbody != null)
		{
			float rightTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
			float leftTrigger = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
			bool isGrounded = Physics.Raycast(GTPlayer.Instance.bodyCollider.transform.position, Vector3.down, 1.5f);
			if (rightTrigger > 0.5f && isGrounded)
			{
				Vector3 targetVelocity = GameNetworkUtilities.GetHeadForward() * 8f;
				_rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, targetVelocity, Time.deltaTime * 3f);
				_rigidbody.useGravity = false;
			}
			else if (leftTrigger > 0.5f)
			{
				_rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime * 5f);
			}
			else
			{
				_rigidbody.useGravity = true;
			}
		}
	}

	private void OnDisable()
	{
		ResetVehicle();
	}

	private void ResetVehicle()
	{
		if (_rigidbody != null)
		{
			_rigidbody.useGravity = true;
		}
	}
}
