// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.PlaySpaceAbuseSimulator
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("PSA Simulator", "Rig", "Play Space Abuse Simulator.", false, 7, ModType.Toggle, false)]
internal sealed class PlaySpaceAbuseSimulator : MonoBehaviour
{
	private void Update()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}

		Rigidbody body = GTPlayer.Instance.GetComponent<Rigidbody>();
		if (!ControllerInputPoller.instance.rightControllerSecondaryButton)
		{
			body.useGravity = true;
			return;
		}

		Vector3 horizontalVelocity = new(body.velocity.x, 0f, body.velocity.z);
		if (horizontalVelocity.magnitude < 8f)
		{
			Vector3 headForward = GTPlayer.Instance.headCollider.transform.forward;
			Vector3 horizontalForward = new(headForward.x, 0f, headForward.z);
			body.AddForce(horizontalForward * 20f, (ForceMode)5);
		}

		body.useGravity = false;
	}

	private void OnDisable()
	{
		if (GTPlayer.Instance != null)
		{
			GTPlayer.Instance.GetComponent<Rigidbody>().useGravity = true;
		}
	}
}
